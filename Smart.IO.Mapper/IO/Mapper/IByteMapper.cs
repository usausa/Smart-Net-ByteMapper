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
        /// <param name="source"></param>
        /// <returns></returns>
        IEnumerable<T> FromBytes<T>(IEnumerable<byte[]> source)
            where T : new();

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        IEnumerable<T> FromBytes<T>(IEnumerable<byte[]> source, Func<T> factory);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        IEnumerable<T> FromBytes<T>(Stream stream)
            where T : new();

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        IEnumerable<T> FromBytes<T>(Stream stream, Func<T> factory);

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
        /// <returns></returns>
        IEnumerable<byte[]> ToBytes<T>(IEnumerable<T> source);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="stream"></param>
        void ToBytes<T>(IEnumerable<T> source, Stream stream);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        T FromString<T>(string str)
            where T : new();

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        T FromString<T>(string str, T target);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        IEnumerable<T> FromStrings<T>(IEnumerable<string> source)
            where T : new();

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        IEnumerable<T> FromStrings<T>(IEnumerable<string> source, Func<T> factory);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        IEnumerable<T> FromStrings<T>(StreamReader stream)
            where T : new();

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        IEnumerable<T> FromStrings<T>(StreamReader stream, Func<T> factory);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        string ToString<T>(T source);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        IEnumerable<string> ToStrings<T>(IEnumerable<T> source);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="stream"></param>
        void ToStrings<T>(IEnumerable<T> source, StreamWriter stream);
    }
}
