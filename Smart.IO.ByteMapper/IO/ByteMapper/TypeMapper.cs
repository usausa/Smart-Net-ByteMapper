namespace Smart.IO.ByteMapper;

using Smart.IO.ByteMapper.Helpers;
using Smart.IO.ByteMapper.Mappers;

internal sealed class TypeMapper<T> : ITypeMapper<T>
{
    private readonly IMapper[] readableMappers;

    private readonly IMapper[] writableMappers;

    private readonly byte filler;

    public Type TargetType { get; }

    public int Size { get; }

    public TypeMapper(Type targetType, int size, byte filler, IReadOnlyList<IMapper> mappers)
    {
        TargetType = targetType;
        Size = size;
        this.filler = filler;
        readableMappers = mappers.Where(static x => x.CanRead).ToArray();
        writableMappers = mappers.Where(static x => x.CanWrite).ToArray();
    }

    public void FromByte(ReadOnlySpan<byte> buffer, object target)
    {
        FromByte(buffer, (T)target);
    }

    public void ToByte(Span<byte> buffer, object target)
    {
        ToByte(buffer, (T)target);
    }

    public void FromByte(ReadOnlySpan<byte> buffer, T target)
    {
        var mappers = readableMappers;
        for (var i = 0; i < mappers.Length; i++)
        {
            mappers[i].Read(buffer, target);
        }
    }

    public void ToByte(Span<byte> buffer, T target)
    {
        if (target == null)
        {
            BytesHelper.Fill(buffer[..Size], filler);
        }
        else
        {
            var mappers = writableMappers;
            for (var i = 0; i < mappers.Length; i++)
            {
                mappers[i].Write(buffer, target);
            }
        }
    }
}
