namespace Smart.IO.ByteMapper;

using Smart.IO.ByteMapper.Mappers;

public sealed class Mapping : IMapping
{
    public Type Type { get; }

    public int Size { get; }

    public byte Filler { get; }

    public IReadOnlyList<IMapper> Mappers { get; }

    public Mapping(Type type, int size, byte filler, IReadOnlyList<IMapper> mappers)
    {
        Type = type;
        Size = size;
        Filler = filler;
        Mappers = mappers;
    }
}
