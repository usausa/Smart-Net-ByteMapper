namespace Smart.IO.Mapper
{
    using System.Collections.Generic;

    public interface IMapperProfile
    {
        IEnumerable<IMapping> ResolveMappings();
    }
}
