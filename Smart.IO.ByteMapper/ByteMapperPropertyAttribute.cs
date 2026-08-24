namespace Smart.IO.ByteMapper;

using System;

// ReSharper disable once UnusedTypeParameter
[AttributeUsage(AttributeTargets.Property)]
public abstract class ByteMapperPropertyAttribute<TConverter> : Attribute
{
    public int Offset { get; }

    protected ByteMapperPropertyAttribute(int offset)
    {
        Offset = offset;
    }
}

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ConverterSupportedTypesAttribute : Attribute
{
    public Type[] Types { get; }

    public ConverterSupportedTypesAttribute(params Type[] types)
    {
        Types = types;
    }
}
