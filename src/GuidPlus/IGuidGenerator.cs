using System;

namespace GuidPlus
{
    /// <summary>
    /// Generator for UUIDs (GUIDs).
    /// </summary>
    public interface IGuidGenerator
    {
        /// <summary>
        /// Generates a UUID.
        /// </summary>
        Guid NewGuid();
    }
}
