namespace Smart.IO.ByteMapper;

[Serializable]
public sealed class ByteMapperException : Exception
{
    public ByteMapperException()
    {
    }

    public ByteMapperException(string message)
        : base(message)
    {
    }

    public ByteMapperException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
