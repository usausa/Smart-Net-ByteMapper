namespace Smart.IO.ByteMapper;

using System;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ByteReaderAttribute : Attribute
{
    public Type? Profile { get; set; }

    public bool ValidateLayout { get; set; } = true;
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class ByteWriterAttribute : Attribute
{
    public Type? Profile { get; set; }

    public bool ValidateLayout { get; set; } = true;
}
