namespace Smart.IO.Mapper.Attributes
{
    using System;
    using System.Reflection;

    using Smart.IO.Mapper.Mappings;

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class AbstractPropertyAttribute : Attribute, IPropertyMappingAttribute
    {
        public string[] Profiles { get; set; }

        public int Offset { get; }

        protected AbstractPropertyAttribute(int offset)
        {
            Offset = offset;
        }

        public abstract bool Match(PropertyInfo pi);

        public IMappingFactory BuildFactory()
        {
            throw new NotImplementedException();
        }
    }
}
