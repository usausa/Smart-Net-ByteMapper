namespace Smart.IO.Mapper
{
    using System;

    using Smart.IO.Mapper.Attributes;

    using Xunit;

    public class ByteMapperConfigAttributeExtensionsTest
    {
        //--------------------------------------------------------------------------------
        // Call
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapCall()
        {
            // Generic

            // Profile
            Assert.NotNull(new ByteMapperConfig()
                .CreateMapByAttribute<SimpleObject>("test")
                .ToByteMapper()
                .Create<SimpleObject>("test"));

            // Validate
            Assert.NotNull(new ByteMapperConfig()
                .CreateMapByAttribute<SimpleObject>(true)
                .ToByteMapper()
                .Create<SimpleObject>());

            // Profile Validate
            Assert.NotNull(new ByteMapperConfig()
                .CreateMapByAttribute<SimpleObject>("test", true)
                .ToByteMapper()
                .Create<SimpleObject>("test"));

            // Type

            // Type
            Assert.NotNull(new ByteMapperConfig()
                .CreateMapByAttribute(typeof(SimpleObject))
                .ToByteMapper()
                .Create<SimpleObject>());

            // Type Profile
            Assert.NotNull(new ByteMapperConfig()
                .CreateMapByAttribute(typeof(SimpleObject), "test")
                .ToByteMapper()
                .Create<SimpleObject>("test"));

            // Type Validate
            Assert.NotNull(new ByteMapperConfig()
                .CreateMapByAttribute(typeof(SimpleObject), true)
                .ToByteMapper()
                .Create<SimpleObject>());

            // Type Profile Validate
            Assert.NotNull(new ByteMapperConfig()
                .CreateMapByAttribute(typeof(SimpleObject), "test", true)
                .ToByteMapper()
                .Create<SimpleObject>("test"));

            // Type-null
            Assert.Throws<ArgumentNullException>(() => new ByteMapperConfig().CreateMapByAttribute((Type)null));

            // Type-invalid
            Assert.Throws<ArgumentException>(() => new ByteMapperConfig().CreateMapByAttribute(typeof(object)));

            // Types

            // Types
            Assert.NotNull(new ByteMapperConfig()
                .CreateMapByAttribute(new[] { typeof(SimpleObject) })
                .ToByteMapper()
                .Create<SimpleObject>());

            // Types Profile
            Assert.NotNull(new ByteMapperConfig()
                .CreateMapByAttribute(new[] { typeof(SimpleObject) }, "test")
                .ToByteMapper()
                .Create<SimpleObject>("test"));

            // Types Validate
            Assert.NotNull(new ByteMapperConfig()
                .CreateMapByAttribute(new[] { typeof(SimpleObject) }, true)
                .ToByteMapper()
                .Create<SimpleObject>());

            // Types Profile Validate
            Assert.NotNull(new ByteMapperConfig()
                .CreateMapByAttribute(new[] { typeof(SimpleObject) }, "test", true)
                .ToByteMapper()
                .Create<SimpleObject>("test"));

            // Types-null
            Assert.Throws<ArgumentNullException>(() => new ByteMapperConfig().CreateMapByAttribute((Type[])null));

            // Null profile is default
            Assert.NotNull(new ByteMapperConfig()
                .CreateMapByAttribute<SimpleObject>(null)
                .ToByteMapper()
                .Create<SimpleObject>());
        }

        //--------------------------------------------------------------------------------
        // Exception
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapByAttributeIsOverlap()
        {
            Assert.Throws<ByteMapperException>(
                () => new ByteMapperConfig().CreateMapByAttribute<OverlapObject>(true).ToByteMapper().Create<OverlapObject>());

            new ByteMapperConfig().CreateMapByAttribute<OverlapObject>(false).ToByteMapper().Create<OverlapObject>();
        }

        [Fact]
        public void MapByAttributeIsNoArray()
        {
            Assert.Throws<ByteMapperException>(
                () => new ByteMapperConfig().CreateMapByAttribute<NoArrayObject>(true).ToByteMapper().Create<NoArrayObject>());
        }

        [Fact]
        public void MapByAttributeIsArrayUnmatched()
        {
            Assert.Throws<ByteMapperException>(
                () => new ByteMapperConfig().CreateMapByAttribute<ArrayUnmatchedObject>(true).ToByteMapper().Create<ArrayUnmatchedObject>());
        }

        [Fact]
        public void MapByAttributeIsUnmatched()
        {
            Assert.Throws<ByteMapperException>(
                () => new ByteMapperConfig().CreateMapByAttribute<UnmatchedObject>(true).ToByteMapper().Create<UnmatchedObject>());
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [Map(0, AutoDelimitter = false)]
        internal class SimpleObject
        {
        }

        [Map(6, AutoDelimitter = false)]
        internal class OverlapObject
        {
            [MapBinary(0)]
            public int IntValue1 { get; set; }

            [MapBinary(2)]
            public int IntValue2 { get; set; }
        }

        [Map(4, AutoDelimitter = false)]
        internal class NoArrayObject
        {
            [MapArray(1)]
            [MapBinary(0)]
            public int ArrayValue { get; set; }
        }

        [Map(4, AutoDelimitter = false)]
        internal class ArrayUnmatchedObject
        {
            [MapArray(1)]
            [MapBinary(0)]
            public string[] ArrayValue { get; set; }
        }

        [Map(1, AutoDelimitter = false)]
        internal class UnmatchedObject
        {
            [MapBinary(0)]
            public string StringValue { get; set; }
        }
    }
}