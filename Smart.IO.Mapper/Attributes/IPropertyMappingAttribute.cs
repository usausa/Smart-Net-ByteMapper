namespace Smart.IO.Mapper.Attributes
{
    using System.Reflection;

    public interface IPropertyMappingAttribute : IMappingAttribute
    {
        bool Match(PropertyInfo pi);
    }
}
