namespace Smart.IO.ByteMapper.Builders;

public sealed class BuilderContextTest
{
    //--------------------------------------------------------------------------------
    // Get
    //--------------------------------------------------------------------------------

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
    // TryGet
    //--------------------------------------------------------------------------------

    [Fact]
    public void TryUseTypeParameter()
    {
        // default
        Assert.True(
            new BuilderContext(
                    null,
                    new Dictionary<string, object> { { "key", 2 } },
                    new Dictionary<string, object> { { "key", null } })
                .TryGetParameter<int>("key", out var value));
        Assert.Equal(0, value);

        // value
        Assert.True(
            new BuilderContext(
                    null,
                    new Dictionary<string, object> { { "key", 2 } },
                    new Dictionary<string, object> { { "key", 1 } })
                .TryGetParameter("key", out int _));
    }

    [Fact]
    public void TryUseGlobalParameter()
    {
        // default
        Assert.True(
            new BuilderContext(
                    null,
                    new Dictionary<string, object> { { "key", null } },
                    new Dictionary<string, object>())
                .TryGetParameter<int>("key", out var value));
        Assert.Equal(0, value);

        // value
        Assert.True(
            new BuilderContext(
                    null,
                    new Dictionary<string, object> { { "key", 2 } },
                    new Dictionary<string, object>())
                .TryGetParameter("key", out value));
        Assert.Equal(2, value);

        // type is unmatch
        Assert.True(
            new BuilderContext(
                    null,
                    new Dictionary<string, object> { { "key", 2 } },
                    new Dictionary<string, object> { { "key", "1" } })
                .TryGetParameter("key", out value));
        Assert.Equal(2, value);
    }

    [Fact]
    public void TryParameterNotFound()
    {
        // not found
        Assert.False(
            new BuilderContext(
                    null,
                    new Dictionary<string, object>(),
                    new Dictionary<string, object>())
                .TryGetParameter<int>("key", out _));

        Assert.False(
            new BuilderContext(
                    null,
                    new Dictionary<string, object> { { "key", "2" } },
                    new Dictionary<string, object> { { "key", "1" } })
                .TryGetParameter<int>("key", out _));
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
