namespace Smart.IO.Mapper.Attributes
{
    using System.Reflection;

    using Smart.IO.Mapper.Mappings;

    public interface IPropertyMappingAttribute : IMappingAttribute
    {
        IMapping CreateMapping(IMappingCreateContext context, PropertyInfo pi);
    }
}
