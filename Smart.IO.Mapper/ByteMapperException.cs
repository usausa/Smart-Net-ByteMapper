namespace Smart.IO.Mapper
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class ByteMapperException : Exception
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

        protected ByteMapperException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
