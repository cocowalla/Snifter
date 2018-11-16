using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Snifter.Filter;
using Snifter.Outputs;

namespace Snifter
{
    public class SocketSniffer
    {
        private const int BUFFER_SIZE = 1024 * 64;
        private const int MAX_RECEIVE = 100;

        private bool isStopping;
        private long packetsObserved;
        private long packetsCaptured;
        private Socket socket;
        private readonly ConcurrentStack<SocketAsyncEventArgs> receivePool;
        private readonly SemaphoreSlim maxReceiveEnforcer = new SemaphoreSlim(MAX_RECEIVE, MAX_RECEIVE);
        private readonly BufferManager bufferManager;
        private readonly BlockingCollection<TimestampedData> outputQueue;
        private readonly Filters<IPPacket> filters;
        private readonly IOutput output;

        public long PacketsObserved => this.packetsObserved;
        public long PacketsCaptured => this.packetsCaptured;

        public SocketSniffer(NetworkInterfaceInfo nic, Filters<IPPacket> filters, IOutput output)
        {
            this.outputQueue = new BlockingCollection<TimestampedData>();
            this.filters = filters;
            this.output = output;

            this.bufferManager = new BufferManager(BUFFER_SIZE, MAX_RECEIVE);
            this.receivePool = new ConcurrentStack<SocketAsyncEventArgs>();
            var endPoint = new IPEndPoint(nic.IPAddress, 0);

            // IPv4
			this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
            this.socket.Bind(endPoint);
			this.socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);

            // Enter promiscuous mode
            try
            {
                this.socket.IOControl(IOControlCode.ReceiveAll, BitConverter.GetBytes(1), new byte[4]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to enter promiscuous mode: {0}", ex);
                throw;
            }
        }

        public void Start()
        {
            // Pre-allocate pool of SocketAsyncEventArgs for receive operations
            for (var i = 0; i < MAX_RECEIVE; i++)
            {
                var socketEventArgs = new SocketAsyncEventArgs();
                socketEventArgs.Completed += (e, args) => Receive(socketEventArgs);

                // Allocate space from the single, shared buffer
                this.bufferManager.AssignSegment(socketEventArgs);

                this.receivePool.Push(socketEventArgs);
            }

            Task.Factory.StartNew(() =>
            {
                // GetConsumingEnumerable() will wait when queue is empty, until CompleteAdding() is called
                foreach (var timestampedData in this.outputQueue.GetConsumingEnumerable())
                {
                    Output(timestampedData);
                }
            });

            Task.Factory.StartNew(StartReceiving);
        }

        public void Stop()
        {
            this.isStopping = true;
        }

        private void EnqueueOutput(TimestampedData timestampedData)
        {
            if (this.isStopping)
            {
                this.outputQueue.CompleteAdding();
                return;
            }

            this.outputQueue.Add(timestampedData);
        }

        private void Output(TimestampedData timestampedData)
        {
            // Only parse the packet header if we need to filter
            if (this.filters.PropertyFilters.Any())
            {
                var packet = new IPPacket(timestampedData.Data);

                if (!this.filters.IsMatch(packet))
                {
                    return;
                }
            }

            this.output.Output(timestampedData);
            Interlocked.Increment(ref this.packetsCaptured);
        }

        private void StartReceiving()
        {
            try
            {
                // Get SocketAsyncEventArgs from pool
                this.maxReceiveEnforcer.Wait();

                if (!this.receivePool.TryPop(out var socketEventArgs))
                {
                    // Because we are controlling access to pooled SocketAsyncEventArgs, this
                    // *should* never happen...
                    throw new Exception("Connection pool exhausted");
                }

                // Returns true if the operation will complete asynchronously, or false if it completed
                // synchronously
                var willRaiseEvent = this.socket.ReceiveAsync(socketEventArgs);

                if (!willRaiseEvent)
                {
                    Receive(socketEventArgs);
                }
            }
            catch (Exception ex)
            {
                // Exceptions while shutting down are expected
                if (!this.isStopping)
                {
                    Console.WriteLine(ex);
                }

                this.socket.Close();
                this.socket = null;
            }
        }

        private void Receive(SocketAsyncEventArgs e)
        {
            // Start a new receive operation straight away, without waiting
            StartReceiving();

            try
            {
                if (e.SocketError != SocketError.Success)
                {
                    if (!this.isStopping)
                    {
                        Console.WriteLine("Socket error: {0}", e.SocketError);
                    }

                    return;
                }

                if (e.BytesTransferred <= 0)
                {
                    return;
                }

                Interlocked.Increment(ref this.packetsObserved);

                // Copy the bytes received into a new buffer
                var buffer = new byte[e.BytesTransferred];
                Buffer.BlockCopy(e.Buffer, e.Offset, buffer, 0, e.BytesTransferred);

                EnqueueOutput(new TimestampedData(DateTime.UtcNow, buffer));
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Socket error: {0}", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}", ex);
            }
            finally
            {
                // Put the SocketAsyncEventArgs back into the pool
                if (!this.isStopping && this.socket != null && this.socket.IsBound)
                {
                    this.receivePool.Push(e);
                    this.maxReceiveEnforcer.Release();
                }
            }
        }
    }
}
