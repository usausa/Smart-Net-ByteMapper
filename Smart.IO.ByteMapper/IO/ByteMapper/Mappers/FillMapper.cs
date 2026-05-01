namespace Smart.IO.ByteMapper.Mappers;

using Smart.IO.ByteMapper.Helpers;

public sealed class FillMapper : IMapper
{
    private readonly int offset;

    private readonly int length;

    private readonly byte filler;

    public bool CanRead => false;

    public bool CanWrite => true;

    public FillMapper(int offset, int length, byte filler)
    {
        this.offset = offset;
        this.length = length;
        this.filler = filler;
    }

    public void Read(ReadOnlySpan<byte> buffer, object target)
    {
        throw new NotSupportedException();
    }

    public void Write(Span<byte> buffer, object target)
    {
        BytesHelper.Fill(buffer.Slice(offset, length), filler);
    }
}
