namespace Smart.IO.ByteMapper
{
    using System;

    using Smart.IO.ByteMapper.Mappers;

    public class Mapping : IMapping
    {
        public Type Type { get; }

        public int Size { get; }

        public byte Filler { get; }

        public IMapper[] Mappers { get; }

        public Mapping(Type type, int size, byte filler, IMapper[] mappers)
        {
            Type = type;
            Size = size;
            Filler = filler;
            Mappers = mappers;
        }
    }
}
