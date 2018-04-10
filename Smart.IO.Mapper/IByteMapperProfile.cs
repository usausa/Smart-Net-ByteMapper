namespace Smart.IO.Mapper
{
    using System.Collections.Generic;

    public interface IByteMapperProfile
    {
        IEnumerable<IMapping> ResolveMappings();
    }
}
