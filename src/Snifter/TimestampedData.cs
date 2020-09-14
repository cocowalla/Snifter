using System;

namespace Snifter
{
	/// <summary>
	/// A raw packet capture with a timestamp of the capture time
	/// </summary>
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
