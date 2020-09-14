using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Snifter.Filter;
using Snifter.Output;
using Snifter.Protocol.Internet;
using Snifter.Protocol.Transport;

namespace Snifter
{
    /// <summary>
    /// Raw socket sniffer
    /// </summary>
    public class SocketSniffer : IDisposable
    {
        private const int BUFFER_SIZE = 1024 * 64;
        private const int MAX_PROCESS_QUEUE = 10_000;

        private volatile bool isStopping;
        private readonly IpPacketParser packetParser = new IpPacketParser(new TransportPacketParser());
        private readonly SocketAsyncEventArgs socketEventArgs = new SocketAsyncEventArgs();
        private readonly Socket socket;
        
        // When packets are captured they are queued here, then parsed, filters and output when dequeued
        private readonly BlockingCollection<TimestampedData> processQueue;
        
        private readonly Filters<IIpPacket> filters;
        private readonly IOutput output;
        private readonly ILogger logger;

        public Statistics Statistics { get; } = new Statistics();

        /// <summary>
        /// Raised when a matching packet has been captured and parsed
        /// </summary>
        public event Action<IIpPacket> PacketCaptured;

        /// <summary>
        /// Create a new raw socket sniffer
        /// </summary>
        /// <param name="nic">Network interface from which to capture packets</param>
        /// <param name="filters">Filters to apply before outputting packets or raising events</param>
        /// <param name="output">An optional output to which matching packets should be written, such as a PCAPNG file</param>
        /// <param name="maxProcessQueue">Maximum size of the packet processing queue. Defaults to 10,000</param>
        /// <param name="logger">An optional logger, used only for logging errors</param>
        public SocketSniffer(NetworkInterfaceInfo nic, Filters<IIpPacket> filters, IOutput output = null, 
            int maxProcessQueue = MAX_PROCESS_QUEUE, ILogger logger = null)
        {
            this.logger = logger ?? NullLogger.Instance;
            this.filters = filters;
            this.output = output;
            this.processQueue = new BlockingCollection<TimestampedData>(maxProcessQueue);

            // Capturing at the IP level is not supported on Linux
            // https://github.com/dotnet/corefx/issues/25115
            // https://github.com/dotnet/corefx/issues/30197
            var protocolType = SystemInformation.IsWindows
                ? ProtocolType.IP
                : ProtocolType.Tcp;

            // IPv4
            var endPoint = new IPEndPoint(nic.IPAddress, 0);
			this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, protocolType);
            this.socket.Bind(endPoint);
			this.socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);

            var buffer = new byte[BUFFER_SIZE];
            this.socketEventArgs.Completed += (e, args) => OnReceived(socketEventArgs);
            this.socketEventArgs.SetBuffer(buffer, 0, BUFFER_SIZE);

            // Enter promiscuous mode on Windows only
            if (SystemInformation.IsWindows)
            {
                EnterPromiscuousMode();
            }
        }

        private void EnterPromiscuousMode()
        {
            try
            {
                this.socket.IOControl(IOControlCode.ReceiveAll, BitConverter.GetBytes(1), new byte[4]);
            }
            catch (Exception ex)
            {
                // Can't sniff raw sockets on Windows without being in promiscuous mode
                throw new InvalidOperationException($"Unable to enter promiscuous mode: {ex.Message}", ex);
            }
        }

        public void Start()
        {
            // Process queued packets that we've read
            _ = Task.Run(() =>
            {
                // GetConsumingEnumerable() will wait when queue is empty, until CompleteAdding() is called
                foreach (var timestampedData in this.processQueue.GetConsumingEnumerable())
                {
                    Process(timestampedData);
                }
            });

            // Read packets and queue them up for processing in this.processQueue
            _ = Task.Run(() => StartReceiving(this.socketEventArgs));
        }

        public void Stop()
        {
            this.isStopping = true;
        }

        // Queue up a captured packet for processing
        private void Enqueue(TimestampedData timestampedData)
        {
            if (this.isStopping)
            {
                this.processQueue.CompleteAdding();
                return;
            }

            var added = this.processQueue.TryAdd(timestampedData);
            this.Statistics.IncrementObserved();

            // Did we add the packet to the processing queue, or was it full?
            if (!added)
            {
                this.Statistics.IncrementDropped();
            }
        }

        // Parse, filter and output a captured packet
        private void Process(TimestampedData timestampedData)
        {
            // Only parse the packet if we need to filter or raise an event
            if (PacketCaptured != null || this.filters.PropertyFilters.Any())
            {
                try
                {
                    var packet = this.packetParser.Parse(timestampedData.Data, timestampedData.Timestamp);

                    if (!this.filters.IsMatch(packet))
                    {
                        return;
                    }

                    try
                    {
                        PacketCaptured?.Invoke(packet);
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogWarning(ex, "Unhandled error in event handler: {ErrorMessage}", ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogWarning(ex, "Unable to parse packet of {Length} bytes received at {Timestamp}: {ErrorMessage}", 
                        timestampedData.Data.Length, timestampedData.Timestamp, ex.Message);
                
                    return;
                }
            }

            this.output?.Output(timestampedData);
            this.Statistics.IncrementCaptured();
        }

        // Start a socket receive operation
        private void StartReceiving(SocketAsyncEventArgs socketEventArgs)
        {
            try
            {
                // If true, the IO operation is still pending, and will complete asynchronously - in that case, the
                // socketEventArgs.Completed event will fire to handle the operation after it completes.
                // If false, the IO operation completed synchronously - in that case, the socketEventArgs.Completed event will not fire, so
                // we call it here
                var operationPending = this.socket.ReceiveAsync(socketEventArgs);

                if (!operationPending)
                {
                    OnReceived(socketEventArgs);
                }
            }
            catch (Exception ex)
            {
                // Exceptions while shutting down are expected
                if (!this.isStopping)
                {
                    throw new InvalidOperationException($"Error while binding to socket: {ex.Message}", ex);
                }

                this.socket.Close();
            }
        }

        // Fired when a socket receive operation has completed
        private void OnReceived(SocketAsyncEventArgs e)
        {
            // Start a new receive operation while we deal with the one that just completed
            //Task.Run(() => StartReceiving());
            
            try
            {
                if (e.SocketError != SocketError.Success)
                {
                    if (!this.isStopping)
                    {
                        throw new InvalidOperationException($"Socket error during receive operation: {e.SocketError}");
                    }

                    return;
                }

                if (e.BytesTransferred <= 0)
                {
                    return;
                }

                // TODO: Use pooled buffers - need to figure out buffer ownership tho, as packets leave our control via the PacketCaptured
                //       event. Probably we copy the pooled buffer to a "new byte[]" if this event is used, *or* we make the user 
                //       responsible for disposing the IIpPackets
                
                // Copy the bytes received into a new buffer
                var buffer = new byte[e.BytesTransferred];
                Buffer.BlockCopy(e.Buffer, e.Offset, buffer, 0, e.BytesTransferred);

                Enqueue(new TimestampedData(DateTime.UtcNow, buffer));
            }
            catch (SocketException ex)
            {
                throw new InvalidOperationException($"Socket error during receive operation: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected error during receive operation: {ex.Message}", ex);
            }
            finally
            {
                // Start the next receive operation
                if (!this.isStopping && this.socket != null && this.socket.IsBound)
                {
                    StartReceiving(e);
                }
            }
        }

        public void Dispose()
        {
            this.socket.Dispose();
            this.processQueue.Dispose();
        }
    }
}
