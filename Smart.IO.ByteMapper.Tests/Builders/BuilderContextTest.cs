namespace Smart.IO.ByteMapper.Builders
{
    using System.Collections.Generic;

    using Xunit;

    public class BuilderContextTest
    {
        [Fact]
        public void UseTypeParameter()
        {
            // default
            Assert.Equal(
                0,
                new BuilderContext(
                        null,
                        new Dictionary<string, object> { { "key", 2 } },
                        new Dictionary<string, object> { { "key", null } })
                    .GetParameter<int>("key"));

            // value
            Assert.Equal(
                1,
                new BuilderContext(
                        null,
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
                new BuilderContext(
                        null,
                        new Dictionary<string, object> { { "key", null } },
                        new Dictionary<string, object>())
                    .GetParameter<int>("key"));

            // value
            Assert.Equal(
                2,
                new BuilderContext(
                        null,
                        new Dictionary<string, object> { { "key", 2 } },
                        new Dictionary<string, object>())
                    .GetParameter<int>("key"));

            // type is unmatch
            Assert.Equal(
                2,
                new BuilderContext(
                        null,
                        new Dictionary<string, object> { { "key", 2 } },
                        new Dictionary<string, object> { { "key", "1" } })
                    .GetParameter<int>("key"));
        }

        [Fact]
        public void ParameterNotFound()
        {
            // not found
            Assert.Throws<ByteMapperException>(() =>
                new BuilderContext(
                        null,
                        new Dictionary<string, object>(),
                        new Dictionary<string, object>())
                    .GetParameter<int>("key"));

            // unmatch
            Assert.Throws<ByteMapperException>(() =>
                new BuilderContext(
                        null,
                        new Dictionary<string, object> { { "key", "2" } },
                        new Dictionary<string, object> { { "key", "1" } })
                    .GetParameter<int>("key"));
        }

        //--------------------------------------------------------------------------------
        // Fix
        //--------------------------------------------------------------------------------

        [Fact]
        public void CoverageFix()
        {
            Assert.Null(new BuilderContext(null, null, null).Components);
        }
    }
}
