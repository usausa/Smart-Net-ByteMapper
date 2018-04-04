namespace Smart.IO.Mapper.Helpers
{
    using System.Collections.Generic;

    using Xunit;

    public class MappingParameterTest
    {
        [Fact]
        public void UseTypeParameter()
        {
            // default
            Assert.Equal(
                0,
                new MappingParameter(
                    new Dictionary<string, object> { { "key", 2 } },
                    new Dictionary<string, object> { { "key", null } })
                    .GetParameter<int>("key"));

            // value
            Assert.Equal(
                1,
                new MappingParameter(
                    new Dictionary<string, object> { { "key", 2 } },
                    new Dictionary<string, object> { { "key", 1 } })
                    .GetParameter<int>("key"));
        }

        [Fact]
        public void UseGlobalParameter()
        {
            // default
            Assert.Equal(
                0,
                new MappingParameter(
                    new Dictionary<string, object> { { "key", null } },
                    new Dictionary<string, object>())
                    .GetParameter<int>("key"));

            // value
            Assert.Equal(
                2,
                new MappingParameter(
                    new Dictionary<string, object> { { "key", 2 } },
                    new Dictionary<string, object>())
                    .GetParameter<int>("key"));

            // type is unmatch
            Assert.Equal(
                2,
                new MappingParameter(
                    new Dictionary<string, object> { { "key", 2 } },
                    new Dictionary<string, object> { { "key", "1" } })
                    .GetParameter<int>("key"));
        }

        [Fact]
        public void ParameterNotFound()
        {
            // not found
            Assert.Throws<ByteMapperException>(() =>
                new MappingParameter(
                    new Dictionary<string, object>(),
                    new Dictionary<string, object>())
                    .GetParameter<int>("key"));

            // unmatch
            Assert.Throws<ByteMapperException>(() =>
                new MappingParameter(
                    new Dictionary<string, object> { { "key", "2" } },
                    new Dictionary<string, object> { { "key", "1" } })
                    .GetParameter<int>("key"));
        }
    }
}
