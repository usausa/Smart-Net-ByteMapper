namespace Smart.IO.Mapper.Converters
{
    using System;

    public class ArrayConverter : IByteConverter
    {
        private readonly int count;

        private readonly Func<int, Array> allocator;

        private readonly IByteConverter elementConverter;

        public int Length => elementConverter.Length * count;

        public ArrayConverter(int count, Func<int, Array> allocator, IByteConverter elementConverter)
        {
            this.count = count;
            this.allocator = allocator;
            this.elementConverter = elementConverter;
        }

        public object Read(byte[] buffer, int index)
        {
            var array = allocator(count);

            for (var i = 0; i < count; i++)
            {
                array.SetValue(elementConverter.Read(buffer, index), i);
                index += elementConverter.Length;
            }

            return array;
        }

        public void Write(byte[] buffer, int index, object value)
        {
            var array = (Array)value;

            for (var i = 0; i < count; i++)
            {
                elementConverter.Write(buffer, index, array.GetValue(i));
                index += elementConverter.Length;
            }
        }
    }
}
