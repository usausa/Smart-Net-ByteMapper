namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Mappings;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public abstract class AbstractTypeMappingAttribute : Attribute, ITypeMappingAttribute
    {
        public string[] Profiles { get; set; }

        public int Offset { get; }

        protected AbstractTypeMappingAttribute(int offset)
        {
            Offset = offset;
        }

        public abstract int CalcSize(Type type);

        public abstract IMapping CreateMapping(IMappingCreateContext context, Type type);
    }
}
