namespace Smart.IO.ByteMapper.Mappers;

#pragma warning disable CA1062
public sealed class ConstantMapper : IMapper
{
    private readonly int offset;

    private readonly int length;

    private readonly byte[] content;

    public bool CanRead => false;

    public bool CanWrite => true;

    public ConstantMapper(int offset, byte[] content)
    {
        this.offset = offset;
        length = content.Length;
        this.content = content;
    }

    public void Read(byte[] buffer, int index, object target)
    {
        throw new NotSupportedException();
    }

    public unsafe void Write(byte[] buffer, int index, object target)
    {
        fixed (byte* pSrc = &content[0])
        fixed (byte* pDst = &buffer[index + offset])
        {
            Buffer.MemoryCopy(pSrc, pDst, length, length);
        }
    }
}
