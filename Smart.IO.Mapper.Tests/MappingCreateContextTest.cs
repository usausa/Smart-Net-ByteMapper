namespace Smart.IO.Mapper
{
    using System.Collections.Generic;

    using Xunit;

    public class MappingCreateContextTest
    {
        [Fact]
        public void UseTypeParameter()
        {
            // default
            Assert.Equal(
                0,
                new MappingCreateContext(
                    new Dictionary<string, object> { { "key", 2 } },
                    new Dictionary<string, object> { { "key", null } },
                    null).GetParameter<int>("key"));

            // value
            Assert.Equal(
                1,
                new MappingCreateContext(
                    new Dictionary<string, object> { { "key", 2 } },
                    new Dictionary<string, object> { { "key", 1 } },
                    null).GetParameter<int>("key"));
        }

        [Fact]
        public void UseGlobalParameter()
        {
            // default
            Assert.Equal(
                0,
                new MappingCreateContext(
                    new Dictionary<string, object> { { "key", null } },
                    new Dictionary<string, object>(),
                    null).GetParameter<int>("key"));

            // value
            Assert.Equal(
                2,
                new MappingCreateContext(
                    new Dictionary<string, object> { { "key", 2 } },
                    new Dictionary<string, object>(),
                    null).GetParameter<int>("key"));

            // type is unmatch
            Assert.Equal(
                2,
                new MappingCreateContext(
                    new Dictionary<string, object> { { "key", 2 } },
                    new Dictionary<string, object> { { "key", "1" } },
                    null).GetParameter<int>("key"));
        }

        [Fact]
        public void ParameterNotFound()
        {
            // not found
            Assert.Throws<ByteMapperException>(() =>
                new MappingCreateContext(
                    new Dictionary<string, object>(),
                    new Dictionary<string, object>(),
                    null).GetParameter<int>("key"));

            // unmatch
            Assert.Throws<ByteMapperException>(() =>
                new MappingCreateContext(
                    new Dictionary<string, object> { { "key", "2" } },
                    new Dictionary<string, object> { { "key", "1" } },
                    null).GetParameter<int>("key"));
        }
    }
}
