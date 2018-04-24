namespace Smart.IO.ByteMapper
{
    using System;

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
                .CreateMapByExpression<SimpleObject>(0, c => c.WithValidation(true))
                .ToMapperFactory()
                .Create<SimpleObject>());

            // Named
            Assert.NotNull(new MapperFactoryConfig()
                .CreateMapByExpression<SimpleObject>(0, "test", c => { })
                .ToMapperFactory()
                .Create<SimpleObject>("test"));

            // Type

            // Type
            Assert.NotNull(new MapperFactoryConfig()
                .CreateMapByExpression(typeof(SimpleObject), 0, c => { })
                .ToMapperFactory()
                .Create<SimpleObject>());

            // Type Named
            Assert.NotNull(new MapperFactoryConfig()
                .CreateMapByExpression(typeof(SimpleObject), 0, "test", c => { })
                .ToMapperFactory()
                .Create<SimpleObject>("test"));

            // Profile

            // Generic

            // Default
            Assert.NotNull(new MapperFactoryConfig()
                .AddProfile(new AnonymousProfile(profile => profile.CreateMapByExpression<SimpleObject>(0, c => { })))
                .ToMapperFactory()
                .Create<SimpleObject>());

            // Named
            Assert.NotNull(new MapperFactoryConfig()
                .AddProfile(new NamedProfile("test", profile => profile.CreateMapByExpression<SimpleObject>(0, c => { })))
                .ToMapperFactory()
                .Create<SimpleObject>("test"));

            // Type

            // Type
            Assert.NotNull(new MapperFactoryConfig()
                .AddProfile(new AnonymousProfile(profile => profile.CreateMapByExpression(typeof(SimpleObject), 0, c => { })))
                .ToMapperFactory()
                .Create<SimpleObject>());

            // Type Named
            Assert.NotNull(new MapperFactoryConfig()
                .AddProfile(new NamedProfile("test", profile => profile.CreateMapByExpression(typeof(SimpleObject), 0, c => { })))
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
