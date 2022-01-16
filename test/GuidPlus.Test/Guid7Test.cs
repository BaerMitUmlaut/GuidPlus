using System;
using System.Globalization;
using System.Numerics;
using Xunit;

namespace GuidPlus.Test
{
    /// <summary>
    /// Unit tests for version 7 UUID generation.
    /// </summary>
    public class Guid7Test
    {
        [Fact]
        public void VersionBitsAreSet()
        {
            var guid = Guid7.NewGuid();

            var versionBits = int.Parse(guid.ToString()[14..15], NumberStyles.HexNumber);
            Assert.Equal(0b0111, versionBits);
        }

        [Fact]
        public void VariantBitsAreSet()
        {
            var guid = Guid7.NewGuid();

            var variantBits = int.Parse(guid.ToString()[19..20], NumberStyles.HexNumber) >> 2;
            Assert.Equal(0b10, variantBits);
        }

        [Fact]
        public void NodeBitsAreSet()
        {
            var node = new byte[] { 0x01, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88 };

            var guid = Guid7.NewGuid(node);

            // First 2 bits are overwritten by variant identifier
            var nodeBits = long.Parse(guid.ToString("N")[17..], NumberStyles.HexNumber);
            Assert.Equal(0x122334455667788, nodeBits);
        }

        [Fact]
        public void NodeBitsAreRandomized()
        {
            var guidA = Guid7.NewGuid();
            var guidB = Guid7.NewGuid();

            var nodeBitsA = long.Parse(guidA.ToString()[24..], NumberStyles.HexNumber);
            var nodeBitsB = long.Parse(guidB.ToString()[24..], NumberStyles.HexNumber);
            Assert.NotEqual(nodeBitsA, nodeBitsB);
        }

        [Fact]
        public void TimestampIsSet()
        {
            var time = new DateTime(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            Guid7._getTime = () => time;

            var guid = Guid7.NewGuid();

            var timestamp = long.Parse(guid.ToString("N")[0..9], NumberStyles.HexNumber);
            var offset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
            Assert.Equal(time, offset.DateTime);
        }

        [Fact]
        public void GuidsAreSequential()
        {
            var guidA = Guid7.NewGuid();
            var guidB = Guid7.NewGuid();

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
            Guid7._getTime = () => time;

            var guidA = Guid7.NewGuid();
            var guidB = Guid7.NewGuid();

            var sequenceA = int.Parse(guidA.ToString()[14..18], NumberStyles.HexNumber) & 0x3fff;
            var sequenceB = int.Parse(guidB.ToString()[14..18], NumberStyles.HexNumber) & 0x3fff;
            Assert.True(sequenceA < sequenceB);
        }
    }
}
