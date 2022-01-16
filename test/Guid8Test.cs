using System;
using System.Globalization;
using System.Numerics;
using Xunit;

namespace GuidPlus.Test
{
    /// <summary>
    /// Unit tests for version 8 UUID generation.
    /// </summary>
    public class Guid8Test
    {
       [Fact]
        public void VersionBitsAreSet()
        {
            var sut = new Guid8.Generator(() => 0xdeadbeef, 32);

            var guid = sut.NewGuid();

            var versionBits = int.Parse(guid.ToString()[14..15], NumberStyles.HexNumber);
            Assert.Equal(0b1000, versionBits);
        }

        [Fact]
        public void VariantBitsAreSet()
        {
            var sut = new Guid8.Generator(() => 0xdeadbeef, 32);

            var guid = sut.NewGuid();

            var variantBits = int.Parse(guid.ToString()[19..20], NumberStyles.HexNumber) >> 2;
            Assert.Equal(0b10, variantBits);
        }

        [Fact]
        public void NodeBitsAreSet32Bit()
        {
            var node = new byte[] { 0x01, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88 };
            var sut = new Guid8.Generator(() => 0xdeadbeef, 32, node);

            var guid = sut.NewGuid();

            // First 2 bits are overwritten by variant identifier
            var nodeBits = long.Parse(guid.ToString("N")[17..], NumberStyles.HexNumber);
            Assert.Equal(0x122334455667788, nodeBits);
        }

        [Fact]
        public void NodeBitsAreSet60Bit()
        {
            var node = new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77 };
            var sut = new Guid8.Generator(() => 0xdeadbeef, 60, node);

            var guid = sut.NewGuid();

            var nodeBits = long.Parse(guid.ToString("N")[18..], NumberStyles.HexNumber);
            Assert.Equal(0x11223344556677, nodeBits);
        }

        [Fact]
        public void NodeBitsAreRandomized()
        {
            var sut = new Guid8.Generator(() => 0xdeadbeef, 32);

            var guidA = sut.NewGuid();
            var guidB = sut.NewGuid();

            var nodeBitsA = long.Parse(guidA.ToString()[24..], NumberStyles.HexNumber);
            var nodeBitsB = long.Parse(guidB.ToString()[24..], NumberStyles.HexNumber);
            Assert.NotEqual(nodeBitsA, nodeBitsB);
        }

        [Fact]
        public void TimestampIsSet8Bit()
        {
            var sut = new Guid8.Generator(() => 0x12, 8);

            var guid = sut.NewGuid();

            var timestamp = int.Parse(guid.ToString("N")[0..2], NumberStyles.HexNumber);
            Assert.Equal(0x12, timestamp);
        }

        [Fact]
        public void TimestampIsSet32Bit()
        {
            var sut = new Guid8.Generator(() => 0xdeadbeef, 32);

            var guid = sut.NewGuid();

            var timestamp = uint.Parse(guid.ToString("N")[0..8], NumberStyles.HexNumber);
            Assert.Equal(0xdeadbeef, timestamp);
        }

        [Fact]
        public void TimestampIsSet60Bit()
        {
            var sut = new Guid8.Generator(() => 0xdeadbeef, 60);

            var guid = sut.NewGuid();

            // Remove version bits
            var timestampStr = guid.ToString("N")[0..16];
            timestampStr = timestampStr.Remove(12, 1);

            var timestamp = long.Parse(timestampStr, NumberStyles.HexNumber);
            Assert.Equal(0xdeadbeef, timestamp);
        }

        [Fact]
        public void GuidsAreSequential()
        {
            var sut = new Guid8.Generator(() => 0xdeadbeef, 32);

            var guidA = sut.NewGuid();
            var guidB = sut.NewGuid();

            // Prefix 0 to ensure numeric value is greater than 0
            var numericA = BigInteger.Parse($"0{guidA:N}", NumberStyles.HexNumber);
            var numericB = BigInteger.Parse($"0{guidB:N}", NumberStyles.HexNumber);
            Assert.True(numericA < numericB);
        }

        [Fact]
        public void SequenceNumberIncreases32()
        {
            var sut = new Guid8.Generator(() => 0xdeadbeef, 32);

            var guidA = sut.NewGuid();
            var guidB = sut.NewGuid();

            var sequenceA = int.Parse(guidA.ToString()[14..18], NumberStyles.HexNumber) & 0x3fff;
            var sequenceB = int.Parse(guidB.ToString()[14..18], NumberStyles.HexNumber) & 0x3fff;
            Assert.True(sequenceA < sequenceB);
        }

        [Fact]
        public void SequenceNumberIncreases60()
        {
            var node = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            var sut = new Guid8.Generator(() => 0xdeadbeef, 60, node);

            var guidA = sut.NewGuid();
            var guidB = sut.NewGuid();

            var sequenceA = int.Parse(guidA.ToString()[19..23], NumberStyles.HexNumber);
            var sequenceB = int.Parse(guidB.ToString()[19..23], NumberStyles.HexNumber);
            Assert.True(sequenceA < sequenceB);
        }
    }
}
