namespace Smart.IO.ByteMapper.Helpers;

using Smart.Collections.Generic;
using Smart.IO.ByteMapper.Mappers;

internal sealed class MapperPosition
{
    public static IComparer<MapperPosition> Comparer => Comparers.Delegate<MapperPosition>(static (x, y) => x.Offset - y.Offset);

    public int Offset { get; }

    public int Size { get; }

    public IMapper Mapper { get; }

    public MapperPosition(int offset, int size, IMapper mapper)
    {
        Offset = offset;
        Size = size;
        Mapper = mapper;
    }
}
