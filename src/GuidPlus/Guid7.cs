using System;
using System.Security.Cryptography;

namespace GuidPlus
{
    /// <summary>
    /// Generator for version 7 UUIDs (GUIDs).
    /// </summary>
    public static partial class Guid7
    {
        internal static Func<ulong> _getTimestamp = () => (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        private static readonly object _sequenceLock = new object();
        private static ulong _lastTimestamp;
        private static int _sequence;

        /// <summary>
        /// Generates a version 7 UUID.
        /// The node bytes are filled with with a cryptographically strong random sequence of bytes.
        /// </summary>
        public static Guid NewGuid()
        {
            var node = new byte[8];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(node);
            }

            return NewGuid(node);
        }

        /// <summary>
        /// Generates a version 7 UUID with the specified node bytes.
        /// </summary>
        /// <param name="node">
        /// 8 node bytes to add to the end of the GUID.
        /// The first two bits of the first byte will by overwritten with <c>0b10</c>.
        /// </param>
        public static Guid NewGuid(byte[] node)
        {
            if (node.Length != 8)
            {
                throw new ArgumentException("Node length must be 8 bytes.", nameof(node));
            }

            ulong unixTsMs;
            int sequence;
            lock (_sequenceLock)
            {
                unixTsMs = _getTimestamp();
                _sequence = unixTsMs > _lastTimestamp ? 0 : _sequence + 1;
                sequence = _sequence;
                _lastTimestamp = unixTsMs;
            }

            var clockSeq = sequence & 0x3fff | 0x7000;

            return new Guid(new[]
            {
                (byte)(unixTsMs >> 16),
                (byte)(unixTsMs >> 24),
                (byte)(unixTsMs >> 32),
                (byte)(unixTsMs >> 40),
                (byte)(unixTsMs),
                (byte)(unixTsMs >> 8),
                (byte)(clockSeq),
                (byte)(clockSeq >> 8),
                (byte)(node[0] & 0x3f | 0x80),
                node[1],
                node[2],
                node[3],
                node[4],
                node[5],
                node[6],
                node[7]
            });
        }
    }
}
