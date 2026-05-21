namespace Smart.IO.ByteMapper;

using System;

public sealed class ByteMapperException : Exception
{
    public ByteMapperException()
    {
    }

    public ByteMapperException(string message)
        : base(message)
    {
    }

    public ByteMapperException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
