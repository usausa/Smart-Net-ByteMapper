namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Mappings;

    public interface ITypeMappingAttribute : IMappingAttribute
    {
        int CalcSize(Type type);

        IMapping CreateMapping(IMappingCreateContext context, Type type);
    }
}
