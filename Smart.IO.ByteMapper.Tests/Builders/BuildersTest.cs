namespace Smart.IO.ByteMapper.Builders
{
    using Smart.IO.ByteMapper.Mock;

    using Xunit;

    public class BuildersTest
    {
        [Fact]
        public void CoverageFix()
        {
            Assert.Equal(0, new BinaryConverterBuilder().CalcSize(typeof(object)));
            Assert.Null(new BinaryConverterBuilder().CreateConverter(new MockBuilderContext(), typeof(object)));

            Assert.Null(new BooleanConverterBuilder().CreateConverter(new MockBuilderContext(), typeof(object)));

            Assert.Null(new ByteConverterBuilder().CreateConverter(new MockBuilderContext(), typeof(object)));

            Assert.Null(new BytesConverterBuilder().CreateConverter(new MockBuilderContext(), typeof(object)));

            Assert.Null(new DateTimeTextConverterBuilder().CreateConverter(new MockBuilderContext(), typeof(object)));

            Assert.Null(new NumberTextConverterBuilder().CreateConverter(new MockBuilderContext(), typeof(object)));

            Assert.Null(new TextConverterBuilder().CreateConverter(new MockBuilderContext(), typeof(object)));
        }
    }
}
