using System;

namespace Snifter
{
    public class TimestampedData
    {
        public DateTime Timestamp { get; private set; }
        public byte[] Data { get; private set; }

        public TimestampedData(DateTime timestamp, byte[] data)
	    {
		    this.Timestamp = timestamp;
            this.Data = data;
	    }
    }
}
