namespace Smart.IO.Mapper.Converters
{
    using System;

    public class ArrayConverter : IMapConverter
    {
        private readonly int count;

        private readonly byte filler;

        private readonly int elementSize;

        private readonly Func<int, Array> allocator;

        private readonly IMapConverter elementConverter;

        public ArrayConverter(Func<int, Array> allocator, int count, byte filler, int elementSize, IMapConverter elementConverter)
        {
            this.allocator = allocator;
            this.count = count;
            this.filler = filler;
            this.elementSize = elementSize;
            this.elementConverter = elementConverter;
        }

        public object Read(byte[] buffer, int index)
        {
            var array = allocator(count);

            for (var i = 0; i < count; i++)
            {
                array.SetValue(elementConverter.Read(buffer, index), i);
                index += elementSize;
            }

            return array;
        }

        public void Write(byte[] buffer, int index, object value)
        {
            if (value == null)
            {
                buffer.Fill(index, count * elementSize, filler);
            }
            else
            {
                var array = (Array)value;

                for (var i = 0; i < count; i++)
                {
                    elementConverter.Write(buffer, index, array.GetValue(i));
                    index += elementSize;
                }
            }
        }
    }
}
