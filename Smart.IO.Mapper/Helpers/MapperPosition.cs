namespace Smart.IO.Mapper.Helpers
{
    using System.Collections.Generic;

    using Smart.Collections.Generic;
    using Smart.IO.Mapper.Mappers;

    internal sealed class MapperPosition
    {
        public static IComparer<MapperPosition> Comparer => Comparers.Delegate<MapperPosition>((x, y) => x.Offset - y.Offset);

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
}
