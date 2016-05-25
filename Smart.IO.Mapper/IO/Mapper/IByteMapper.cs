namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    ///
    /// </summary>
    public interface IByteMapper
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        T FromByte<T>(byte[] buffer)
            where T : new();

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        T FromByte<T>(byte[] buffer, T target);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        IEnumerable<T> FromByte<T>(Stream stream)
            where T : new();

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        IEnumerable<T> FromByte<T>(Stream stream, Func<T> factory);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        byte[] ToByte<T>(T source);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="stream"></param>
        void ToByte<T>(IEnumerable<T> source, Stream stream);
    }
}
