namespace Smart.IO.ByteMapper
{
    using System;

    using Smart.IO.ByteMapper.Attributes;

    using Xunit;

    public class AttributeExtensionsTest
    {
        //--------------------------------------------------------------------------------
        // Call
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByAttributeCall()
        {
            // Generic

            // Named
            Assert.NotNull(new MapperFactoryConfig()
                .CreateMapByAttribute<SimpleObject>("test")
                .ToMapperFactory()
                .Create<SimpleObject>("test"));

            // Validation
            Assert.NotNull(new MapperFactoryConfig()
                .CreateMapByAttribute<SimpleObject>(true)
                .ToMapperFactory()
                .Create<SimpleObject>());

            // Named Validation
            Assert.NotNull(new MapperFactoryConfig()
                .CreateMapByAttribute<SimpleObject>("test", true)
                .ToMapperFactory()
                .Create<SimpleObject>("test"));

            // Type

            // Type
            Assert.NotNull(new MapperFactoryConfig()
                .CreateMapByAttribute(typeof(SimpleObject))
                .ToMapperFactory()
                .Create<SimpleObject>());

            // Type Named
            Assert.NotNull(new MapperFactoryConfig()
                .CreateMapByAttribute(typeof(SimpleObject), "test")
                .ToMapperFactory()
                .Create<SimpleObject>("test"));

            // Type Validation
            Assert.NotNull(new MapperFactoryConfig()
                .CreateMapByAttribute(typeof(SimpleObject), true)
                .ToMapperFactory()
                .Create<SimpleObject>());

            // Type Named Validation
            Assert.NotNull(new MapperFactoryConfig()
                .CreateMapByAttribute(typeof(SimpleObject), "test", true)
                .ToMapperFactory()
                .Create<SimpleObject>("test"));

            // Type-null
            Assert.Throws<ArgumentNullException>(() => new MapperFactoryConfig().CreateMapByAttribute((Type)null));

            // Type-invalid
            Assert.Throws<ArgumentException>(() => new MapperFactoryConfig().CreateMapByAttribute(typeof(object)));

            // Types

            // Types
            Assert.NotNull(new MapperFactoryConfig()
                .CreateMapByAttribute(new[] { typeof(SimpleObject) })
                .ToMapperFactory()
                .Create<SimpleObject>());

            // Types Named
            Assert.NotNull(new MapperFactoryConfig()
                .CreateMapByAttribute(new[] { typeof(SimpleObject) }, "test")
                .ToMapperFactory()
                .Create<SimpleObject>("test"));

            // Types Validation
            Assert.NotNull(new MapperFactoryConfig()
                .CreateMapByAttribute(new[] { typeof(SimpleObject) }, true)
                .ToMapperFactory()
                .Create<SimpleObject>());

            // Types Named Validation
            Assert.NotNull(new MapperFactoryConfig()
                .CreateMapByAttribute(new[] { typeof(SimpleObject) }, "test", true)
                .ToMapperFactory()
                .Create<SimpleObject>("test"));

            // Types-null
            Assert.Throws<ArgumentNullException>(() => new MapperFactoryConfig().CreateMapByAttribute((Type[])null));

            // Null name is default
            Assert.NotNull(new MapperFactoryConfig()
                .CreateMapByAttribute<SimpleObject>(null)
                .ToMapperFactory()
                .Create<SimpleObject>());

            // Profile

            // Generic

            // Default
            Assert.NotNull(new MapperFactoryConfig()
                .AddProfile(new AnonymousProfile(profile => profile.CreateMapByAttribute<SimpleObject>()))
                .ToMapperFactory()
                .Create<SimpleObject>());

            // Named
            Assert.NotNull(new MapperFactoryConfig()
                .AddProfile(new NamedProfile("test", profile => profile.CreateMapByAttribute<SimpleObject>()))
                .ToMapperFactory()
                .Create<SimpleObject>("test"));

            // Validation
            Assert.NotNull(new MapperFactoryConfig()
                .AddProfile(new AnonymousProfile(profile => profile.CreateMapByAttribute<SimpleObject>(true)))
                .ToMapperFactory()
                .Create<SimpleObject>());

            // Named Validation
            Assert.NotNull(new MapperFactoryConfig()
                .AddProfile(new NamedProfile("test", profile => profile.CreateMapByAttribute<SimpleObject>(true)))
                .ToMapperFactory()
                .Create<SimpleObject>("test"));

            // Type

            // Type
            Assert.NotNull(new MapperFactoryConfig()
                .AddProfile(new AnonymousProfile(profile => profile.CreateMapByAttribute(typeof(SimpleObject))))
                .ToMapperFactory()
                .Create<SimpleObject>());

            // Type Named
            Assert.NotNull(new MapperFactoryConfig()
                .AddProfile(new NamedProfile("test", profile => profile.CreateMapByAttribute(typeof(SimpleObject))))
                .ToMapperFactory()
                .Create<SimpleObject>("test"));

            // Type Validation
            Assert.NotNull(new MapperFactoryConfig()
                .AddProfile(new AnonymousProfile(profile => profile.CreateMapByAttribute(typeof(SimpleObject), true)))
                .ToMapperFactory()
                .Create<SimpleObject>());

            // Type Named Validation
            Assert.NotNull(new MapperFactoryConfig()
                .AddProfile(new NamedProfile("test", profile => profile.CreateMapByAttribute(typeof(SimpleObject), true)))
                .ToMapperFactory()
                .Create<SimpleObject>("test"));

            // Type-null
            Assert.Throws<ArgumentNullException>(() => new MapperFactoryConfig().AddProfile(new AnonymousProfile(profile => profile.CreateMapByAttribute((Type)null))));

            // Type-invalid
            Assert.Throws<ArgumentException>(() => new MapperFactoryConfig().AddProfile(new AnonymousProfile(profile => profile.CreateMapByAttribute(typeof(object)))));

            // Types
            Assert.NotNull(new MapperFactoryConfig()
                .AddProfile(new AnonymousProfile(profile => profile.CreateMapByAttribute(new[] { typeof(SimpleObject) })))
                .ToMapperFactory()
                .Create<SimpleObject>());

            // Types Validation
            Assert.NotNull(new MapperFactoryConfig()
                .AddProfile(new AnonymousProfile(profile => profile.CreateMapByAttribute(new[] { typeof(SimpleObject) }, true)))
                .ToMapperFactory()
                .Create<SimpleObject>());

            // Type-null
            Assert.Throws<ArgumentNullException>(() => new MapperFactoryConfig().AddProfile(new AnonymousProfile(profile => profile.CreateMapByAttribute((Type[])null))));
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [Map(0, UseDelimitter = false)]
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
