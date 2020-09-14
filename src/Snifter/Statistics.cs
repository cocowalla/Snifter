using System.Threading;

namespace Snifter
{
    /// <summary>
    /// Holds statistics about a packet capture session
    /// </summary>
    public class Statistics
    {
        private long packetsObserved;
        private long packetsCaptured;
        private long packetsDropped;
        private int buffersInUse;

        public long PacketsObserved => this.packetsObserved;
        public long PacketsCaptured => this.packetsCaptured;
        public long PacketsDropped => this.packetsDropped;
        public int BuffersInUse => this.buffersInUse;

        public void IncrementObserved()
            => Interlocked.Increment(ref this.packetsObserved);
        
        public void IncrementCaptured()
            => Interlocked.Increment(ref this.packetsCaptured);

        public void IncrementDropped()
            => Interlocked.Increment(ref this.packetsDropped);
        
        public void SetBuffersInUse(int curentBuffersInUse)
            => Interlocked.Exchange(ref this.buffersInUse, curentBuffersInUse);
    }
}
