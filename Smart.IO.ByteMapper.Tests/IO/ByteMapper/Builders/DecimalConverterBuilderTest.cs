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
            new DecimalConverterBuilder { Length = 19, Scale = 1 }.CreateConverter(new MockBuilderContext(), typeof(decimal));
            Assert.Throws<InvalidOperationException>(() =>
                new DecimalConverterBuilder { Length = 20, Scale = 1 }.CreateConverter(new MockBuilderContext(), typeof(decimal)));

            new DecimalConverterBuilder { Length = 18 }.CreateConverter(new MockBuilderContext(), typeof(decimal));
            Assert.Throws<InvalidOperationException>(() =>
                new DecimalConverterBuilder { Length = 19 }.CreateConverter(new MockBuilderContext(), typeof(decimal)));

            new DecimalConverterBuilder { Length = 23, UseGrouping = true, GroupingSize = 3 }.CreateConverter(new MockBuilderContext(), typeof(decimal));
            Assert.Throws<InvalidOperationException>(() =>
                new DecimalConverterBuilder { Length = 24, UseGrouping = true, GroupingSize = 3 }.CreateConverter(new MockBuilderContext(), typeof(decimal)));
        }
    }
}
