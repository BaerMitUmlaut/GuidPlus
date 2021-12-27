﻿using System;
using System.Linq;
using System.Security.Cryptography;

namespace GuidPlus
{
    /// <summary>
    /// Generator for version 6 UUIDs (GUIDs).
    /// </summary>
    public static class Guid6
    {
        private static readonly DateTime _gregorianEpoch = new DateTime(1582, 10, 15, 0, 0, 0, DateTimeKind.Utc);
        private static DateTime _lastClock;
        private static int _sequence;
        internal static Func<DateTime> _getTime = () => DateTime.UtcNow;

        /// <summary>
        /// Generates a version 6 UUID.
        /// The node bytes are filled with with a cryptographically strong random sequence of bytes.
        /// </summary>
        public static Guid NewGuid()
        {
            var node = new byte[6];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(node);
            }

            return NewGuid(node);
        }

        /// <summary>
        /// Generates a version 6 UUID with the specified node bytes.
        /// </summary>
        /// <param name="node">6 node bytes to add to the end of the GUID.</param>
        public static Guid NewGuid(byte[] node)
        {
            if (node.Length != 6)
            {
                throw new ArgumentException("Node length must be 6 bytes", nameof(node));
            }

            var clock = _getTime();
            _sequence = clock > _lastClock ? 0 : _sequence + 1;
            _lastClock = clock;

            var timestamp = (clock - _gregorianEpoch).Ticks;
            var timeHigh = (int)(timestamp >> 28);
            var timeMid = (short)(timestamp >> 12);
            var timeLow = (short)(timestamp & 0x0fff | 0x6000);
            var clockSeq = (short)(_sequence & 0x3fff | 0x8000);

            return new Guid(
                timeHigh,
                timeMid,
                timeLow,
                BitConverter.GetBytes(clockSeq).Reverse().Concat(node).ToArray()
            );
        }
    }
}
