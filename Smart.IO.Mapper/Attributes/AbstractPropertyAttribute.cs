namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Converters;

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class AbstractPropertyAttribute : Attribute, IPropertyMappingAttribute
    {
        public int Offset { get; }

        protected AbstractPropertyAttribute(int offset)
        {
            Offset = offset;
        }

        public abstract int CalcSize(Type type);

        public abstract IByteConverter CreateConverter(IMappingCreateContext context, Type type);
    }
}
