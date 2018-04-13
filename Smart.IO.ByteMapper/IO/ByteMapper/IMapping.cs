namespace Smart.IO.ByteMapper
{
    using System;

    using Smart.IO.ByteMapper.Mappers;

    public interface IMapping
    {
        Type Type { get; }

        int Size { get; }

        byte Filler { get; }

        IMapper[] Mappers { get; }
    }
}
