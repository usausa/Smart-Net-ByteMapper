namespace Smart.IO.ByteMapper;

using System;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public abstract class ByteMapperPropertyAttribute : Attribute
{
    public int Offset { get; }

    protected ByteMapperPropertyAttribute(int offset)
    {
        Offset = offset;
    }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public abstract class ByteMapperConverterAttribute<TConverter> : ByteMapperPropertyAttribute
{
    protected ByteMapperConverterAttribute(int offset)
        : base(offset)
    {
    }
}
