namespace Smart.IO.ByteMapper.Converters;

using Smart.IO.ByteMapper.Helpers;

internal sealed class BytesConverter : IMapConverter
{
    private readonly int length;

    private readonly byte filler;

    public BytesConverter(int length, byte filler)
    {
        this.length = length;
        this.filler = filler;
    }

    public object Read(byte[] buffer, int index)
    {
        return buffer.AsSpan(index, length).ToArray();
    }

    public void Write(byte[] buffer, int index, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer, index, length, filler);
        }
        else
        {
            var bytes = (byte[])value;
            BytesHelper.CopyBytes(bytes, buffer, index, length, Padding.Right, filler);
        }
    }
}
