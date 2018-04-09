namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Builders;

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class AbstractMapMemberAttribute : Attribute, IMapMemberAttribute
    {
        public int Offset { get; }

        protected AbstractMapMemberAttribute(int offset)
        {
            Offset = offset;
        }

        public abstract IMapConverterBuilder GetConverterBuilder();
    }
}
