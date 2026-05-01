namespace Smart.IO.ByteMapper.Converters;

using Smart.IO.ByteMapper.Helpers;

internal sealed class ArrayConverter : IMapConverter
{
    private readonly int length;

    private readonly byte filler;

    private readonly int elementSize;

    private readonly Func<int, Array> allocator;

    private readonly IMapConverter elementConverter;

    public ArrayConverter(Func<int, Array> allocator, int length, byte filler, int elementSize, IMapConverter elementConverter)
    {
        this.allocator = allocator;
        this.length = length;
        this.filler = filler;
        this.elementSize = elementSize;
        this.elementConverter = elementConverter;
    }

    public object Read(ReadOnlySpan<byte> buffer)
    {
        var array = allocator(length);

        for (var i = 0; i < length; i++)
        {
            array.SetValue(elementConverter.Read(buffer[(i * elementSize)..]), i);
        }

        return array;
    }

    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..(length * elementSize)], filler);
        }
        else
        {
            var array = (Array)value;

            for (var i = 0; i < array.Length; i++)
            {
                elementConverter.Write(buffer[(i * elementSize)..], array.GetValue(i));
            }

            if (array.Length < length)
            {
                BytesHelper.Fill(buffer.Slice(array.Length * elementSize, (length - array.Length) * elementSize), filler);
            }
        }
    }
}
