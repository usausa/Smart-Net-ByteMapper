namespace Smart.IO.ByteMapper.Expressions
{
    using Smart.Functional;

    using Xunit;

    public class BooleanExpressionTest
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
        public void MapByBinaryExpression()
        {
            var mapperFactory = new MapperFactoryConfig()
                .DefaultDelimiter(null)
                .DefaultFiller(Filler)
                .DefaultTrueValue(True)
                .DefaultFalseValue(False)
                .Also(config =>
                {
                    config
                        .CreateMapByExpression<BooleanExpressionObject>(4)
                        .ForMember(x => x.BooleanValue, c => c.Boolean())
                        .ForMember(x => x.NullableBooleanValue, c => c.Boolean())
                        .ForMember(x => x.CustomBooleanValue, c => c.Boolean(Yes, No))
                        .ForMember(x => x.CustomNullableBooleanValue, c => c.Boolean(Yes, No, Filler));
                })
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
