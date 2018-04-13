namespace Smart.IO.ByteMapper
{
    using System;

    using Smart.Functional;

    using Xunit;

    public class ExpressionExtensionsTest
    {
        //--------------------------------------------------------------------------------
        // Call
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByExpressionCall()
        {
            // Generic

            // Validation
            Assert.NotNull(new MapperFactoryConfig()
                .Also(config => { config.CreateMapByExpression<SimpleObject>(0).WithValidation(true); })
                .ToMapperFactory()
                .Create<SimpleObject>());

            // Named
            Assert.NotNull(new MapperFactoryConfig()
                .Also(config => { config.CreateMapByExpression<SimpleObject>(0, "test"); })
                .ToMapperFactory()
                .Create<SimpleObject>("test"));

            // Type

            // Type
            Assert.NotNull(new MapperFactoryConfig()
                .Also(config => { config.CreateMapByExpression(typeof(SimpleObject), 0); })
                .ToMapperFactory()
                .Create<SimpleObject>());

            // Type Named
            Assert.NotNull(new MapperFactoryConfig()
                .Also(config => { config.CreateMapByExpression(typeof(SimpleObject), 0, "test"); })
                .ToMapperFactory()
                .Create<SimpleObject>("test"));

            // Profile

            // Generic

            // Default
            Assert.NotNull(new MapperFactoryConfig()
                .AddProfile(new AnonymousProfile(profile => profile.CreateMapByExpression<SimpleObject>(0)))
                .ToMapperFactory()
                .Create<SimpleObject>());

            // Named
            Assert.NotNull(new MapperFactoryConfig()
                .AddProfile(new NamedProfile("test", profile => profile.CreateMapByExpression<SimpleObject>(0)))
                .ToMapperFactory()
                .Create<SimpleObject>("test"));

            // Type

            // Type
            Assert.NotNull(new MapperFactoryConfig()
                .AddProfile(new AnonymousProfile(profile => profile.CreateMapByExpression(typeof(SimpleObject), 0)))
                .ToMapperFactory()
                .Create<SimpleObject>());

            // Type Named
            Assert.NotNull(new MapperFactoryConfig()
                .AddProfile(new NamedProfile("test", profile => profile.CreateMapByExpression(typeof(SimpleObject), 0)))
                .ToMapperFactory()
                .Create<SimpleObject>("test"));
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        internal class SimpleObject
        {
        }

        internal class AnonymousProfile : MapperProfile
        {
            public AnonymousProfile(Action<MapperProfile> action)
            {
                action(this);
            }
        }

        internal class NamedProfile : MapperProfile
        {
            public NamedProfile(string name, Action<MapperProfile> action)
                : base(name)
            {
                action(this);
            }
        }
    }
}
