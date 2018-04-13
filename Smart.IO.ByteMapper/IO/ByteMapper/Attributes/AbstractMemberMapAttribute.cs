namespace Smart.IO.ByteMapper.Attributes
{
    using System;

    using Smart.IO.ByteMapper.Builders;

    [AttributeUsage(AttributeTargets.Property)]
    public abstract class AbstractMemberMapAttribute : Attribute
    {
        public int Offset { get; }

        protected AbstractMemberMapAttribute(int offset)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            Offset = offset;
        }

        public abstract IMapConverterBuilder GetConverterBuilder();
    }
}
