namespace Smart.IO.ByteMapper;

using System;
using System.Collections.Generic;

using Smart.IO.ByteMapper.Mappers;

public class Mapping : IMapping
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
