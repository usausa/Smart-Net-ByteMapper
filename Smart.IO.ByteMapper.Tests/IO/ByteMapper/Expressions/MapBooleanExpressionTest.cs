namespace Smart.IO.ByteMapper.Expressions
{
    using Xunit;

    public class MapBooleanExpressionTest
    {
        private const byte Filler = 0x00;

        private const byte True = (byte)'1';

        private const byte False = (byte)'0';

        private const byte Yes = (byte)'Y';

        private const byte No = (byte)'N';

        //--------------------------------------------------------------------------------
        // Expression
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByBooleanExpression()
        {
            var mapperFactory = new MapperFactoryConfig()
                .DefaultDelimiter(null)
                .DefaultFiller(Filler)
                .DefaultTrueValue(True)
                .DefaultFalseValue(False)
                .CreateMapByExpression<BooleanExpressionObject>(4, config => config
                    .ForMember(x => x.BooleanValue, m => m.Boolean())
                    .ForMember(x => x.NullableBooleanValue, m => m.Boolean())
                    .ForMember(x => x.CustomBooleanValue, m => m.Boolean(Yes, No))
                    .ForMember(x => x.CustomNullableBooleanValue, m => m.Boolean(Yes, No, Filler)))
                .ToMapperFactory();
            var mapper = mapperFactory.Create<BooleanExpressionObject>();

            var buffer = new byte[mapper.Size];
            var obj = new BooleanExpressionObject();

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
        // Helper
        //--------------------------------------------------------------------------------

        internal class BooleanExpressionObject
        {
            public bool BooleanValue { get; set; }

            public bool? NullableBooleanValue { get; set; }

            public bool CustomBooleanValue { get; set; }

            public bool? CustomNullableBooleanValue { get; set; }
        }
    }
}
