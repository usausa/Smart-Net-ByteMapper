namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Xunit;

    public class MapBooleanAttributeTest
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
        public void MapByBooleanAttribute()
        {
            var byteMapper = new ByteMapperConfig()
                .DefaultDelimiter(null)
                .DefaultFiller(Filler)
                .DefaultTrueValue(True)
                .DefaultFalseValue(False)
                .CreateMapByAttribute<BooleanAttributeObject>()
                .ToByteMapper();
            var mapper = byteMapper.Create<BooleanAttributeObject>();

            var buffer = new byte[mapper.Size];
            var obj = new BooleanAttributeObject();

            // Write
            mapper.ToByte(buffer, 0, obj);

            Assert.Equal(new[] { False, Filler, No, Filler }, buffer);

            // Read
            mapper.FromByte(new[] { True, True, Yes, Yes }, 0, obj);

            Assert.True(obj.BooleanValue);
            Assert.True(obj.NullableBooleanValue);
            Assert.True(obj.CustomBooleanValue);
            Assert.True(obj.CustomNullableBooleanValue);

            // Read default
            mapper.FromByte(new[] { Filler, Filler, Filler, Filler }, 0, obj);

            Assert.False(obj.BooleanValue);
            Assert.Null(obj.NullableBooleanValue);
            Assert.False(obj.CustomBooleanValue);
            Assert.Null(obj.CustomNullableBooleanValue);
        }

        //--------------------------------------------------------------------------------
        // Fix
        //--------------------------------------------------------------------------------

        [Fact]
        public void CoverageFix()
        {
            var attribute = new MapBooleanAttribute(0);

            Assert.Throws<NotSupportedException>(() => attribute.TrueValue);
            Assert.Throws<NotSupportedException>(() => attribute.FalseValue);
            Assert.Throws<NotSupportedException>(() => attribute.NullValue);

            Assert.Null(attribute.CreateConverter(null, null, typeof(object)));
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [Map(4)]
        internal class BooleanAttributeObject
        {
            [MapBoolean(0)]
            public bool BooleanValue { get; set; }

            [MapBoolean(1)]
            public bool? NullableBooleanValue { get; set; }

            [MapBoolean(2, TrueValue = Yes, FalseValue = No)]
            public bool CustomBooleanValue { get; set; }

            [MapBoolean(3, TrueValue = Yes, FalseValue = No, NullValue = Filler)]
            public bool? CustomNullableBooleanValue { get; set; }
        }
    }
}
