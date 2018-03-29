namespace Smart.IO.Mapper
{
    using System.Text;

    using Smart.IO.Mapper.Attributes;

    using Xunit;

    public class AttributeTest
    {
        [Fact]
        public void MapByAttribute()
        {
            var byteMapper = new ByteMapperConfig()
                .MapByAttribute<Data>()
                .ToByteMapper();

            var mapper = byteMapper.Create<Data>();

            var data = new Data();
            mapper.FromByte(Encoding.ASCII.GetBytes("  1234abc "), data);

            Assert.Equal(1234, data.IntValue);
            Assert.Equal("abc", data.StringValue);
        }

        [Map(Size)]
        internal class Data
        {
            private const int IntValueLength = 6;
            private const int StringValueLength = 4;
            private const int DelimitterLength = 2;

            private const int IntValueOffset = 0;
            private const int StringValueOffset = IntValueOffset + IntValueLength;
            private const int DelimitterOffset = StringValueOffset + StringValueLength;

            private const int Size = DelimitterOffset + DelimitterLength;

            [NumberText(IntValueOffset, Length = IntValueLength)]
            public int IntValue { get; set; }

            [String(StringValueOffset, Length = StringValueLength)]
            public string StringValue { get; set; }
        }
    }
}
