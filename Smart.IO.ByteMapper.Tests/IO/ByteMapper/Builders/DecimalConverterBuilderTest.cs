namespace Smart.IO.ByteMapper.Builders
{
    using System;

    using Smart.IO.ByteMapper.Mock;

    using Xunit;

    public class DecimalConverterBuilderTest
    {
        [Fact]
        public void CoverageFix()
        {
            new DecimalConverterBuilder { Length = 18 }.CreateConverter(new MockBuilderContext(), typeof(decimal));
            Assert.Throws<InvalidOperationException>(() =>
                new DecimalConverterBuilder { Length = 19 }.CreateConverter(new MockBuilderContext(), typeof(decimal)));
        }
    }
}
