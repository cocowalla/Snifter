using System;
using System.Net.Sockets;

namespace Snifter
{
    /// <summary>
    /// Fixed pool of buffers for use by async socket connections, to avoid connections creating their
    /// own buffers (which leads to a lot of pinned buffers, causing heap fragmentation)
    /// </summary>
    public class BufferManager
    {
        private readonly byte[] buffer;
        private readonly int segmentSize;
        private int nextOffset;

        public BufferManager(int segmentSize, int numSegments)
        {
            this.segmentSize = segmentSize;
            this.buffer = new byte[segmentSize * numSegments];
        }

        public void AssignSegment(SocketAsyncEventArgs e)
        {
            if ((this.nextOffset + this.segmentSize) > this.buffer.Length)
            {
                throw new IndexOutOfRangeException("Buffer exhausted");
            }

            e.SetBuffer(this.buffer, this.nextOffset, this.segmentSize);
            this.nextOffset += this.segmentSize;
        }
    }
}
