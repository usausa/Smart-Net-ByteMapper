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

// Specifies the property types that a converter attribute supports.
// The source generator emits SBM0008 when the mapped property type is not in the list.
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ConverterSupportedTypesAttribute : Attribute
{
    public Type[] Types { get; }

    public ConverterSupportedTypesAttribute(params Type[] types)
    {
        Types = types;
    }
}
