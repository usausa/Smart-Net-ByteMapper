namespace Smart.IO.ByteMapper;

using Smart.IO.ByteMapper.Mappers;

public interface IMapping
{
    Type Type { get; }

    int Size { get; }

    byte Filler { get; }

    IReadOnlyList<IMapper> Mappers { get; }
}
