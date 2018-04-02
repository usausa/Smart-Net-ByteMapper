namespace Smart.IO.Mapper
{
    using Smart.IO.Mapper.Attributes;

    using Xunit;

    public class TypeMapperTest
    {
        //--------------------------------------------------------------------------------
        // From
        //--------------------------------------------------------------------------------

        [Fact]
        public void FromByteVariations()
        {
            var mapper = CreateTypeMapper<TargetObject>();
            var buffer = new byte[mapper.Size];

            // Default index
            mapper.FromByte(buffer, new TargetObject());

            // With create
            Assert.NotNull(mapper.FromByte(buffer));

            // With create index
            Assert.NotNull(mapper.FromByte(buffer, 0));

            // Mutliple
            Assert.Single(mapper.FromByteMultiple(buffer));

            // Mutliple index
            Assert.Single(mapper.FromByteMultiple(buffer, 0));

            // Mutliple factory
            Assert.Single(mapper.FromByteMultiple(buffer, () => new TargetObject()));

            // Mutliple index factory
            Assert.Single(mapper.FromByteMultiple(buffer, 0, () => new TargetObject()));

            // TODO
        }

        //--------------------------------------------------------------------------------
        // Fix
        //--------------------------------------------------------------------------------

        [Fact]
        public void TypeMapperTargetType()
        {
            Assert.Equal(typeof(TargetObject), CreateTypeMapper<TargetObject>().TargetType);
        }

        [Fact]
        public void TypeMapperNotExists()
        {
            Assert.Throws<ByteMapperException>(() => new ByteMapperConfig().ToByteMapper().Create<object>());
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        private ITypeMapper<T> CreateTypeMapper<T>()
        {
            var byteMapper = new ByteMapperConfig()
                .MapByAttribute<TargetObject>()
                .DefaultDelimiter(null)
                .ToByteMapper();
            return byteMapper.Create<T>();
        }

        [Map(4, AutoDelimitter = false)]
        internal class TargetObject
        {
            [MapBinary(0)]
            public int IntValue { get; set; }
        }
    }
}
