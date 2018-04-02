namespace Smart.IO.Mapper
{
    using System;

    using Smart.Functional;
    using Smart.IO.Mapper.Mappings;

    using Xunit;

    public class CoverageFixTest
    {
        [Fact]
        public void TypeMapperNotExists()
        {
            Assert.Throws<ByteMapperException>(() => new ByteMapperConfig().ToByteMapper().Create<object>());
        }

        [Fact]
        public void UnreadableMapping()
        {
            Assert.Throws<NotSupportedException>(() => new ConstMapping(0, new byte[0]).Also(x => x.Read(null, 0, null)));
            Assert.Throws<NotSupportedException>(() => new FillMapping(0, 0, 0x00).Also(x => x.Read(null, 0, null)));
        }
    }
}
