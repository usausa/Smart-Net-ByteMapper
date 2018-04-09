namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Builders;

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class AbstractMemberMapAttribute : Attribute
    {
        public int Offset { get; }

        protected AbstractMemberMapAttribute(int offset)
        {
            Offset = offset;
        }

        public abstract IMapConverterBuilder GetConverterBuilder();
    }
}
