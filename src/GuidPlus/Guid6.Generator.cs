using System;

namespace GuidPlus
{
    /// <summary>
    /// Generator for version 6 UUIDs (GUIDs).
    /// </summary>
    public static partial class Guid6
    {
        /// <summary>
        /// Preconfigured generator for version 6 UUIDs (GUIDs).
        /// </summary>
        public class Generator : IGuidGenerator
        {
            private readonly byte[] _node;

            /// <summary>
            /// Initializes a new generator for version 6 UUIDs with the specified node bytes.
            /// </summary>
            /// <param name="node">6 node bytes to add to the end of the GUID.</param>
            public Generator(byte[] node)
            {
                if (node.Length != 6)
                {
                    throw new ArgumentException("Node length must be 6 bytes", nameof(node));
                }

                _node = node;
            }

            /// <inheritdoc />
            public Guid NewGuid()
            {
                return Guid6.NewGuid(_node);
            }
        }
    }
}
