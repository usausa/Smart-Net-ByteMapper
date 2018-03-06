namespace Smart.IO.MapperOld
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class ByteMapperException : Exception
    {
        /// <summary>
        ///
        /// </summary>
        public ByteMapperException()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        public ByteMapperException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ByteMapperException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ByteMapperException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
