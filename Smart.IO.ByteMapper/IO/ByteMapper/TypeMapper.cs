namespace Smart.IO.ByteMapper;

using Smart.IO.ByteMapper.Helpers;
using Smart.IO.ByteMapper.Mappers;

internal class TypeMapper<T> : ITypeMapper<T>
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
        readableMappers = mappers.Where(x => x.CanRead).ToArray();
        writableMappers = mappers.Where(x => x.CanWrite).ToArray();
    }

    public void FromByte(byte[] buffer, int index, object target)
    {
        FromByte(buffer, index, (T)target);
    }

    public void ToByte(byte[] buffer, int index, object target)
    {
        ToByte(buffer, index, (T)target);
    }

    public void FromByte(byte[] buffer, int index, T target)
    {
        var mappers = readableMappers;
        for (var i = 0; i < mappers.Length; i++)
        {
            mappers[i].Read(buffer, index, target);
        }
    }

    public void ToByte(byte[] buffer, int index, T target)
    {
        if (target == null)
        {
            BytesHelper.Fill(buffer, index, Size, filler);
        }
        else
        {
            var mappers = writableMappers;
            for (var i = 0; i < mappers.Length; i++)
            {
                mappers[i].Write(buffer, index, target);
            }
        }
    }
}
