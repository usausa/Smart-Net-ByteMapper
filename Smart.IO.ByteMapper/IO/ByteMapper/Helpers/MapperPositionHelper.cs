namespace Smart.IO.ByteMapper.Helpers;

using Smart.IO.ByteMapper.Mappers;

internal static class MapperPositionHelper
{
    public static void Layout(List<MapperPosition> list, int size, string typeName, bool validation, byte[] delimiter, byte? filler)
    {
        if ((delimiter != null) && (delimiter.Length > 0))
        {
            var offset = size - delimiter.Length;
            list.Add(new MapperPosition(offset, delimiter.Length, new ConstantMapper(offset, delimiter)));
        }

        list.Sort(MapperPosition.Comparer);

        var fillers = new List<MapperPosition>();
        for (var i = 0; i < list.Count + 1; i++)
        {
            var start = i < list.Count ? list[i].Offset : list.Count == 0 ? 0 : size;
            var end = i < list.Count ? list[i].Offset + list[i].Size : list.Count == 0 ? 0 : size;
            var next = i < list.Count - 1 ? list[i + 1].Offset : size;

            if (validation && (end > next))
            {
                throw new ByteMapperException($"Range overlap. type=[{typeName}], range=[{start}..{end}], next=[{next}]");
            }

            if (filler.HasValue && (end < next))
            {
                var length = next - end;
                fillers.Add(new MapperPosition(end, length, new FillMapper(end, length, filler.Value)));
            }
        }

        if (fillers.Count > 0)
        {
            list.AddRange(fillers);
            list.Sort(MapperPosition.Comparer);
        }
    }
}
