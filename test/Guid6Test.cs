using System;
using System.Globalization;
using System.Numerics;
using Xunit;

namespace GuidPlus.Test
{
    /// <summary>
    /// Unit tests for version 6 UUID generation.
    /// </summary>
    public class Guid6Test
    {
        [Fact]
        public void VersionBitsAreSet()
        {
            var guid = Guid6.NewGuid();

            var versionBits = int.Parse(guid.ToString()[14..15], NumberStyles.HexNumber);
            Assert.Equal(0b0110, versionBits);
        }

        [Fact]
        public void VariantBitsAreSet()
        {
            var guid = Guid6.NewGuid();

            var variantBits = int.Parse(guid.ToString()[19..20], NumberStyles.HexNumber) >> 2;
            Assert.Equal(0b10, variantBits);
        }

        [Fact]
        public void NodeBitsAreSet()
        {
            var node = new byte[] { 0xaa, 0xbb, 0xcc, 0xdd, 0xee, 0xff };

            var guid = Guid6.NewGuid(node);

            var nodeBits = long.Parse(guid.ToString()[24..], NumberStyles.HexNumber);
            Assert.Equal(0xaabbccddeeff, nodeBits);
        }

        [Fact]
        public void NodeBitsAreRandomized()
        {
            var guidA = Guid6.NewGuid();
            var guidB = Guid6.NewGuid();

            var nodeBitsA = long.Parse(guidA.ToString()[24..], NumberStyles.HexNumber);
            var nodeBitsB = long.Parse(guidB.ToString()[24..], NumberStyles.HexNumber);
            Assert.NotEqual(nodeBitsA, nodeBitsB);
        }

        [Fact]
        public void GuidsAreSequential()
        {
            var guidA = Guid6.NewGuid();
            var guidB = Guid6.NewGuid();

            // Prefix 0 to ensure numeric value is greater than 0
            var numericA = BigInteger.Parse($"0{guidA:N}", NumberStyles.HexNumber);
            var numericB = BigInteger.Parse($"0{guidB:N}", NumberStyles.HexNumber);
            Assert.True(numericA < numericB);
        }

        [Fact]
        public void SequenceNumberIncreases()
        {
            // Freeze time
            var time = DateTime.Now;
            Guid6.GetTime = () => time;

            var guidA = Guid6.NewGuid();
            var guidB = Guid6.NewGuid();

            var sequenceA = int.Parse(guidA.ToString()[19..23], NumberStyles.HexNumber) & 0x3fff;
            var sequenceB = int.Parse(guidB.ToString()[19..23], NumberStyles.HexNumber) & 0x3fff;
            Assert.True(sequenceA < sequenceB);
        }
    }
}