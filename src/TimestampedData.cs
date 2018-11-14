using System;

namespace Snifter
{
    public class TimestampedData
    {
        public DateTime Timestamp { get; }
        public byte[] Data { get; }

        public TimestampedData(DateTime timestamp, byte[] data)
	    {
		    this.Timestamp = timestamp;
            this.Data = data;
	    }
    }
}
