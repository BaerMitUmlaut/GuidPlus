using System;
using System.Linq;
using System.Security.Cryptography;

namespace GuidPlus
{
    /// <summary>
    /// Generator for version 6 UUIDs (GUIDs).
    /// </summary>
    public static class Guid6
    {
        private static DateTime GregorianEpoch { get; } = new DateTime(1582, 10, 15, 0, 0, 0, DateTimeKind.Utc);
        private static DateTime LastClock { get; set; }
        private static RandomNumberGenerator RandomNumberGenerator { get; } = RandomNumberGenerator.Create();
        private static int Sequence { get; set; }
        internal static Func<DateTime> GetTime { get; set; } = () => DateTime.UtcNow;

        /// <summary>
        /// Generates a version 6 UUID.
        /// The node bytes are filled with with a cryptographically strong random sequence of bytes.
        /// </summary>
        public static Guid NewGuid()
        {
            var node = new byte[6];
            RandomNumberGenerator.GetBytes(node);

            return NewGuid(node);
        }

        /// <summary>
        /// Generates a version 6 UUID with the specified node bytes.
        /// </summary>
        /// <param name="node">Node bytes to add to the end of the GUID.</param>
        public static Guid NewGuid(byte[] node)
        {
            if (node.Length != 6)
            {
                throw new ArgumentException("Node length must be 6 bytes", nameof(node));
            }

            var clock = GetTime();
            Sequence = clock > LastClock ? 0 : Sequence + 1;
            LastClock = clock;

            var timestamp = (clock - GregorianEpoch).Ticks;
            var timeHigh = (int)(timestamp >> 28);
            var timeMid = (short)(timestamp >> 12);
            var timeLow = (short)(timestamp & 0x0fff | 0x6000);
            var clockSeq = (short)(Sequence & 0x3fff | 0x8000);

            return new Guid(
                timeHigh,
                timeMid,
                timeLow,
                BitConverter.GetBytes(clockSeq).Reverse().Concat(node).ToArray()
            );
        }
    }
}
