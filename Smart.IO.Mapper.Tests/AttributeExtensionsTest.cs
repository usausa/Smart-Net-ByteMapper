namespace Smart.IO.Mapper
{
    using System;

    using Smart.IO.Mapper.Attributes;

    using Xunit;

    public class AttributeExtensionsTest
    {
        //--------------------------------------------------------------------------------
        // Call
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapCall()
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
        }

        //--------------------------------------------------------------------------------
        // Exception
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByAttributeIsOverlap()
        {
            Assert.Throws<ByteMapperException>(
                () => new MapperFactoryConfig().CreateMapByAttribute<OverlapObject>(true).ToMapperFactory().Create<OverlapObject>());

            new MapperFactoryConfig().CreateMapByAttribute<OverlapObject>(false).ToMapperFactory().Create<OverlapObject>();
        }

        [Fact]
        public void MapByAttributeIsNoArray()
        {
            Assert.Throws<ByteMapperException>(
                () => new MapperFactoryConfig().CreateMapByAttribute<NoArrayObject>(true).ToMapperFactory().Create<NoArrayObject>());
        }

        [Fact]
        public void MapByAttributeIsArrayUnmatched()
        {
            Assert.Throws<ByteMapperException>(
                () => new MapperFactoryConfig().CreateMapByAttribute<ArrayUnmatchedObject>(true).ToMapperFactory().Create<ArrayUnmatchedObject>());
        }

        [Fact]
        public void MapByAttributeIsUnmatched()
        {
            Assert.Throws<ByteMapperException>(
                () => new MapperFactoryConfig().CreateMapByAttribute<UnmatchedObject>(true).ToMapperFactory().Create<UnmatchedObject>());
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [Map(0, UseDelimitter = false)]
        internal class SimpleObject
        {
        }

        [Map(6, UseDelimitter = false)]
        internal class OverlapObject
        {
            [MapBinary(0)]
            public int IntValue1 { get; set; }

            [MapBinary(2)]
            public int IntValue2 { get; set; }
        }

        [Map(4, UseDelimitter = false)]
        internal class NoArrayObject
        {
            [MapArray(1)]
            [MapBinary(0)]
            public int ArrayValue { get; set; }
        }

        [Map(4, UseDelimitter = false)]
        internal class ArrayUnmatchedObject
        {
            [MapArray(1)]
            [MapBinary(0)]
            public string[] ArrayValue { get; set; }
        }

        [Map(1, UseDelimitter = false)]
        internal class UnmatchedObject
        {
            [MapBinary(0)]
            public string StringValue { get; set; }
        }
    }
}