using System;

namespace GuidPlus
{
    /// <summary>
    /// Generator for version 7 UUIDs (GUIDs).
    /// </summary>
    public static partial class Guid7
    {
        /// <summary>
        /// Preconfigured generator for version 7 UUIDs (GUIDs).
        /// </summary>
        public class Generator : IGuidGenerator
        {
            private readonly byte[] _node;

            /// <summary>
            /// Initializes a new generator for version 7 UUIDs with the specified node bytes.
            /// </summary>
            /// <param name="node">
            /// 8 node bytes to add to the end of the GUID.
            /// The first two bits of the first byte will by overwritten with <c>0b10</c>.
            /// </param>
            public Generator(byte[] node)
            {
                if (node.Length != 8)
                {
                    throw new ArgumentException("Node length must be 8 bytes.", nameof(node));
                }

                _node = node;
            }

            /// <inheritdoc />
            public Guid NewGuid()
            {
                return Guid7.NewGuid(_node);
            }
        }
    }
}
