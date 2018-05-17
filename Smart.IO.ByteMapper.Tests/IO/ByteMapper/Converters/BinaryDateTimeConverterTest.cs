//namespace Smart.IO.ByteMapper.Converters
//{
//    using System;

//    using Smart.IO.ByteMapper.Mock;

//    using Xunit;

//    public class BigEndianDateTimeBinaryConverterTest
//    {
//        private const int Offset = 1;

//        private static readonly DateTime Value = new DateTime(1);

//        private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, new byte[]
//        {
//            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01
//        });

//        private readonly IMapConverter converter = BigEndianDateTimeBinaryConverter.Default;

//        [Fact]
//        public void ReadToBigEndianDateTimeBinary()
//        {
//            Assert.Equal(Value, (DateTime)converter.Read(ValueBytes, Offset));
//        }

//        [Fact]
//        public void WriteBigEndianDateTimeBinaryToBuffer()
//        {
//            var buffer = new byte[8 + Offset];
//            converter.Write(buffer, Offset, Value);

//            Assert.Equal(ValueBytes, buffer);
//        }
//    }

//    public class LittleEndianDateTimeBinaryConverterTest
//    {
//        private const int Offset = 1;

//        private static readonly DateTime Value = new DateTime(1);

//        private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, new byte[]
//        {
//            0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
//        });

//        private readonly IMapConverter converter = LittleEndianDateTimeBinaryConverter.Default;

//        [Fact]
//        public void ReadToBigEndianDateTimeBinary()
//        {
//            Assert.Equal(Value, (DateTime)converter.Read(ValueBytes, Offset));
//        }

//        [Fact]
//        public void WriteBigEndianDateTimeBinaryToBuffer()
//        {
//            var buffer = new byte[8 + Offset];
//            converter.Write(buffer, Offset, Value);

//            Assert.Equal(ValueBytes, buffer);
//        }
//    }

//    public class BigEndianDateTimeOffsetBinaryConverterTest
//    {
//        private const int Offset = 1;

//        private static readonly DateTimeOffset Value = new DateTimeOffset(new DateTime(1, DateTimeKind.Utc));

//        private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, new byte[]
//        {
//            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01
//        });

//        private readonly IMapConverter converter = BigEndianDateTimeOffsetBinaryConverter.Default;

//        [Fact]
//        public void ReadToBigEndianDateTimeOffsetBinary()
//        {
//            Assert.Equal(Value, (DateTimeOffset)converter.Read(ValueBytes, Offset));
//        }

//        [Fact]
//        public void WriteBigEndianDateTimeOffsetBinaryToBuffer()
//        {
//            var buffer = new byte[8 + Offset];
//            converter.Write(buffer, Offset, Value);

//            Assert.Equal(ValueBytes, buffer);
//        }
//    }

//    public class LittleEndianDateTimeOffsetBinaryConverterTest
//    {
//        private const int Offset = 1;

//        private static readonly DateTimeOffset Value = new DateTimeOffset(new DateTime(1), TimeSpan.Zero);

//        private static readonly byte[] ValueBytes = TestBytes.Offset(Offset, new byte[]
//        {
//            0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
//        });

//        private readonly IMapConverter converter = LittleEndianDateTimeOffsetBinaryConverter.Default;

//        [Fact]
//        public void ReadToBigEndianDateTimeOffsetBinary()
//        {
//            Assert.Equal(Value, (DateTimeOffset)converter.Read(ValueBytes, Offset));
//        }

//        [Fact]
//        public void WriteBigEndianDateTimeOffsetBinaryToBuffer()
//        {
//            var buffer = new byte[8 + Offset];
//            converter.Write(buffer, Offset, Value);

//            Assert.Equal(ValueBytes, buffer);
//        }
//    }
//}
