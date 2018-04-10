namespace Smart.IO.ByteMapper
{
    using System.Collections.Generic;

    public interface IMapperProfile
    {
        IEnumerable<IMapping> ResolveMappings();
    }
}
