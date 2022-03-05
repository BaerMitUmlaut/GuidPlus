using System;
using System.Security.Cryptography;

namespace GuidPlus
{
    /// <summary>
    /// Generator for version 8 UUIDs (GUIDs).
    /// </summary>
    public static class Guid8
    {
        /// <summary>
        /// Preconfigured generator for version 8 UUIDs (GUIDs).
        /// </summary>
        public class Generator : IGuidGenerator
        {
            private readonly Func<ulong> _timeSource;
            private readonly int _timeSize;
            private readonly Func<byte[]> _nodeGenerator;
            private readonly object _sequenceLock = new object();
            private ulong _lastClock;
            private int _sequence;

            /// <summary>
            /// Initializes a new generator for version 8 UUIDs using the given timestamp provider.
            /// </summary>
            /// <param name="timeSource">Timestamp provider function.</param>
            /// <param name="timeSize">Size of provided timestamp in bit (up to 60).</param>
            public Generator(Func<ulong> timeSource, int timeSize)
                : this(timeSource, timeSize, () => GenerateRandomNode(timeSize)) { }

            /// <summary>
            /// Initializes a new generator for version 8 UUIDs using the given timestamp provider
            /// and node bytes.
            /// </summary>
            /// <param name="timeSource">Timestamp provider function.</param>
            /// <param name="timeSize">Size of provided timestamp in bit (up to 60).</param>
            /// <param name="node">
            /// Node bytes to add to the end of the GUID. Timestamps using up to 48 bits require 7
            /// node bytes, larger timestamps require 8 node bytes. The first two bits of the first
            /// byte will by overwritten.
            /// </param>
            public Generator(Func<ulong> timeSource, int timeSize, byte[] node)
                : this(timeSource, timeSize, () => node)
            {
                if (timeSize > 48 && node.Length != 7)
                {
                    throw new ArgumentException(
                        "Node length must be 7 bytes for timestamps using up to 48 bits.",
                        nameof(node)
                    );
                }

                if (timeSize <= 48 && node.Length != 8)
                {
                    throw new ArgumentException(
                        "Node length must be 8 bytes for timestamps larger than 48 bits.",
                        nameof(node)
                    );
                }
            }

            /// <summary>
            /// Initializes a new generator for version 8 UUIDs using the given timestamp provider
            /// and node byte gennerator.
            /// </summary>
            /// <param name="timeSource">Timestamp provider function.</param>
            /// <param name="timeSize">Size of provided timestamp in bit (up to 60).</param>
            /// <param name="nodeGenerator">Node provider function.</param>
            private Generator(Func<ulong> timeSource, int timeSize, Func<byte[]> nodeGenerator)
            {
                if (timeSize > 60)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(timeSize),
                        "Timestamps larger than 60 bits are not supported."
                    );
                }

                if (timeSize < 1)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(timeSize),
                        "Timestamps cannot be smaller than 1 bit."
                    );
                }

                _timeSource = timeSource ?? throw new ArgumentNullException(nameof(timeSource));
                _timeSize = timeSize;
                _nodeGenerator = nodeGenerator;
            }

            /// <summary>
            /// Generates random node bytes for the given timestamp size.
            /// </summary>
            /// <param name="timeSize">Size of provided timestamp in bit (up to 60).</param>
            private static byte[] GenerateRandomNode(int timeSize)
            {
                var nodeSize = timeSize > 48 ? 7 : 8;
                var node = new byte[nodeSize];
                using (var randomNumberGenerator = RandomNumberGenerator.Create())
                {
                    randomNumberGenerator.GetBytes(node);
                }

                return node;
            }

            /// <inheritdoc />
            public Guid NewGuid()
            {
                ulong clock;
                int sequence;
                lock (_sequenceLock)
                {
                    clock = _timeSource();
                    _sequence = clock > _lastClock ? 0 : _sequence + 1;
                    sequence = _sequence;
                    _lastClock = clock;
                }

                clock = clock << 64 - _timeSize;
                var timestamp32 = (int)(clock >> 32);
                var timestamp48 = (short)(clock >> 16);
                var timeOrSeq = _timeSize <= 48
                    ? (short)(sequence & 0x0fff | 0x8000)
                    : (short)(clock >> 4 & 0x0fff | 0x8000);
                var node = _nodeGenerator();

                if (_timeSize <= 48)
                {
                    return new Guid(
                        timestamp32,
                        timestamp48,
                        timeOrSeq,
                        (byte)(node[0] & 0x3f | 0x80),
                        node[1],
                        node[2],
                        node[3],
                        node[4],
                        node[5],
                        node[6],
                        node[7]
                    );
                }
                else
                {
                    return new Guid(
                        timestamp32,
                        timestamp48,
                        timeOrSeq,
                        (byte)(sequence >> 2 & 0x3f | 0x80),
                        (byte)(node[0] & 0x3f | sequence << 6),
                        node[1],
                        node[2],
                        node[3],
                        node[4],
                        node[5],
                        node[6]
                    );
                }
            }
        }
    }
}
