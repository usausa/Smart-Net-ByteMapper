namespace Smart.IO.Mapper
{
    using Smart.IO.Mapper.Attributes;

    using Xunit;

    public class TypeMapperTest
    {
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
