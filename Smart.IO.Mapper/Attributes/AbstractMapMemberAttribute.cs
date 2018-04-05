namespace Smart.IO.Mapper.Attributes
{
    using System;
    using Smart.ComponentModel;

    using Smart.IO.Mapper.Converters;
    using Smart.IO.Mapper.Helpers;

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class AbstractMapMemberAttribute : Attribute, IMapMemberAttribute
    {
        public int Offset { get; }

        protected AbstractMapMemberAttribute(int offset)
        {
            Offset = offset;
        }

        public abstract int CalcSize(Type type);

        public abstract IByteConverter CreateConverter(IComponentContainer components, IMappingParameter parameters, Type type);
    }
}
