namespace Smart.IO.Mapper.Attributes
{
    using System.Globalization;
    using System.Text;

    using Xunit;

    public class TypeDefaultAttributeTest
    {
        //--------------------------------------------------------------------------------
        // Attribute
        //--------------------------------------------------------------------------------

        [Fact]
        public void MapUseTypeDefaultAttribute()
        {
            var mapperFactory = new MapperFactoryConfig()
                .DefaultDelimiter(null)
                .DefaultEncoding(Encoding.ASCII)
                .DefaultNumberProvider(CultureInfo.InvariantCulture)
                .DefaultNumberPadding(Padding.Left)
                .DefaultNumberFiller(0x20)
                .DefaultNumberStyle(NumberStyles.Integer)
                .CreateMapByAttribute<TypeDefaultAttributeObject>()
                .ToMapperFactory();
            var mapper = mapperFactory.Create<TypeDefaultAttributeObject>();

            var buffer = new byte[mapper.Size];
            var obj = new TypeDefaultAttributeObject { IntValue = 1 };

            // Write
            mapper.ToByte(buffer, 0, obj);

            Assert.Equal(Encoding.ASCII.GetBytes("1___"), buffer);
        }

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [Map(4)]
        [TypeNumberPadding(Padding.Right)]
        [TypeNumberFiller((byte)'_')]
        internal class TypeDefaultAttributeObject
        {
            [MapNumberText(0, 4)]
            public int IntValue { get; set; }
        }
    }
}
