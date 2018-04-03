namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Mock;

    using Xunit;

    public class MapBoolAttributeTest
    {
        private const byte Filler = 0x00;

        private const byte True = (byte)'1';

        private const byte False = (byte)'0';

        private const byte Yes = (byte)'Y';

        private const byte No = (byte)'N';

        //--------------------------------------------------------------------------------
        // Attribute
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByBoolAttribute()
        {
            var byteMapper = new ByteMapperConfig()
                .DefaultDelimiter(null)
                .DefaultFiller(Filler)
                .DefaultTrueValue(True)
                .DefaultFalseValue(False)
                .MapByAttribute<BoolAttributeObject>()
                .ToByteMapper();
            var mapper = byteMapper.Create<BoolAttributeObject>();

            var buffer = new byte[mapper.Size];
            var obj = new BoolAttributeObject();

            // Write
            mapper.ToByte(buffer, 0, obj);

            Assert.Equal(new[] { False, Filler, No, Filler }, buffer);

            // Read
            mapper.FromByte(new[] { True, True, Yes, Yes }, 0, obj);

            Assert.True(obj.BoolValue);
            Assert.True(obj.NullableBoolValue);
            Assert.True(obj.CustomBoolValue);
            Assert.True(obj.CustomNullableBoolValue);

            // Read default
            mapper.FromByte(new[] { Filler, Filler, Filler, Filler }, 0, obj);

            Assert.False(obj.BoolValue);
            Assert.Null(obj.NullableBoolValue);
            Assert.False(obj.CustomBoolValue);
            Assert.Null(obj.CustomNullableBoolValue);
        }

        //--------------------------------------------------------------------------------
        // Fix
        //--------------------------------------------------------------------------------

        [Fact]
        public void CoverageFix()
        {
            var attribute = new MapBoolAttribute(0);

            Assert.Throws<NotSupportedException>(() => attribute.TrueValue);
            Assert.Throws<NotSupportedException>(() => attribute.FalseValue);
            Assert.Throws<NotSupportedException>(() => attribute.NullValue);

            Assert.Null(attribute.CreateConverter(new MockMappingCreateContext(), typeof(object)));
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [Map(4)]
        internal class BoolAttributeObject
        {
            [MapBool(0)]
            public bool BoolValue { get; set; }

            [MapBool(1)]
            public bool? NullableBoolValue { get; set; }

            [MapBool(2, TrueValue = Yes, FalseValue = No)]
            public bool CustomBoolValue { get; set; }

            [MapBool(3, TrueValue = Yes, FalseValue = No, NullValue = Filler)]
            public bool? CustomNullableBoolValue { get; set; }
        }
    }
}
