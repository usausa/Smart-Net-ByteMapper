namespace Smart.IO.Mapper.Mappings
{
    using System;

    using Smart.Functional;

    using Xunit;

    public class CoverageFixTest
    {
        [Fact]
        public void UnreadableMapping()
        {
            Assert.Throws<NotSupportedException>(() => new ConstMapping(0, new byte[0]).Also(x => x.Read(null, 0, null)));
            Assert.Throws<NotSupportedException>(() => new FillMapping(0, 0, 0x00).Also(x => x.Read(null, 0, null)));
        }
    }
}
