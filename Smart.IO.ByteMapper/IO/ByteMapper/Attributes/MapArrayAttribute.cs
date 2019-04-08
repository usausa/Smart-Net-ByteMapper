namespace Smart.IO.ByteMapper.Attributes
{
    using System;

    using Smart.IO.ByteMapper.Builders;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MapArrayAttribute : Attribute
    {
        private readonly ArrayConverterBuilder builder = new ArrayConverterBuilder();

        public byte Filler
        {
            get => throw new NotSupportedException();
            set => builder.Filler = value;
        }

        public MapArrayAttribute(int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            builder.Length = length;
        }

        public ArrayConverterBuilder GetArrayConverterBuilder() => builder;
    }
}
