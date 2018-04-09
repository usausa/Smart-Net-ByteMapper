namespace Smart.IO.Mapper.Attributes
{
    using System;

    using Smart.IO.Mapper.Builders;

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
            builder.Length = length;
        }

        public ArrayConverterBuilder GetArrayConverterBuilder()
        {
            return builder;
        }
    }
}
