namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Mappings;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public abstract class AbstractTypeMappingAttribute : Attribute, IMappingAttribute
    {
        public string[] Profiles { get; set; }

        public int Offset { get; }

        protected AbstractTypeMappingAttribute(int offset)
        {
            Offset = offset;
        }

        public abstract IMapping Create(IMappingCreateContext context);
    }
}
