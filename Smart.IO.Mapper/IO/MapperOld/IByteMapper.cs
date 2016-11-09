namespace Smart.IO.MapperOld
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    ///
    /// </summary>
    public interface IByteMapper
    {
        //--------------------------------------------------------------------------------
        // FromByte
        //--------------------------------------------------------------------------------

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
        /// <param name="profile"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        T FromByte<T>(string profile, byte[] buffer)
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
        /// <param name="profile"></param>
        /// <param name="buffer"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        T FromByte<T>(string profile, byte[] buffer, T target);

        //--------------------------------------------------------------------------------
        // FromBytes
        //--------------------------------------------------------------------------------

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
        /// <param name="profile"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        IEnumerable<T> FromBytes<T>(string profile, IEnumerable<byte[]> source)
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
        /// <param name="profile"></param>
        /// <param name="source"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        IEnumerable<T> FromBytes<T>(string profile, IEnumerable<byte[]> source, Func<T> factory);

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
        /// <param name="profile"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        IEnumerable<T> FromBytes<T>(string profile, Stream stream)
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
        /// <param name="profile"></param>
        /// <param name="stream"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        IEnumerable<T> FromBytes<T>(string profile, Stream stream, Func<T> factory);

        //--------------------------------------------------------------------------------
        // ToByte
        //--------------------------------------------------------------------------------

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
        /// <param name="profile"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        byte[] ToByte<T>(string profile, T source);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="appendDelimiter"></param>
        /// <returns></returns>
        byte[] ToByte<T>(T source, bool appendDelimiter);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="profile"></param>
        /// <param name="source"></param>
        /// <param name="appendDelimiter"></param>
        /// <returns></returns>
        byte[] ToByte<T>(string profile, T source, bool appendDelimiter);

        //--------------------------------------------------------------------------------
        // ToBytes
        //--------------------------------------------------------------------------------

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
        /// <param name="profile"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        IEnumerable<byte[]> ToBytes<T>(string profile, IEnumerable<T> source);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="appendDelimiter"></param>
        /// <returns></returns>
        IEnumerable<byte[]> ToBytes<T>(IEnumerable<T> source, bool appendDelimiter);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="profile"></param>
        /// <param name="source"></param>
        /// <param name="appendDelimiter"></param>
        /// <returns></returns>
        IEnumerable<byte[]> ToBytes<T>(string profile, IEnumerable<T> source, bool appendDelimiter);

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
        /// <param name="profile"></param>
        /// <param name="source"></param>
        /// <param name="stream"></param>
        void ToBytes<T>(string profile, IEnumerable<T> source, Stream stream);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="appendDelimiter"></param>
        /// <param name="stream"></param>
        void ToBytes<T>(IEnumerable<T> source, bool appendDelimiter, Stream stream);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="profile"></param>
        /// <param name="source"></param>
        /// <param name="appendDelimiter"></param>
        /// <param name="stream"></param>
        void ToBytes<T>(string profile, IEnumerable<T> source, bool appendDelimiter, Stream stream);

        //--------------------------------------------------------------------------------
        // FromString
        //--------------------------------------------------------------------------------

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
        /// <param name="profile"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        T FromString<T>(string profile, string str)
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
        /// <param name="profile"></param>
        /// <param name="str"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        T FromString<T>(string profile, string str, T target);

        //--------------------------------------------------------------------------------
        // FromStrings
        //--------------------------------------------------------------------------------

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
        /// <param name="profile"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        IEnumerable<T> FromStrings<T>(string profile, IEnumerable<string> source)
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
        /// <param name="profile"></param>
        /// <param name="source"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        IEnumerable<T> FromStrings<T>(string profile, IEnumerable<string> source, Func<T> factory);

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
        /// <param name="profile"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        IEnumerable<T> FromStrings<T>(string profile, StreamReader stream)
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
        /// <param name="profile"></param>
        /// <param name="stream"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        IEnumerable<T> FromStrings<T>(string profile, StreamReader stream, Func<T> factory);

        //--------------------------------------------------------------------------------
        // ToString
        //--------------------------------------------------------------------------------

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
        /// <param name="profile"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        string ToString<T>(string profile, T source);

        //--------------------------------------------------------------------------------
        // ToStrings
        //--------------------------------------------------------------------------------

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
        /// <param name="profile"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        IEnumerable<string> ToStrings<T>(string profile, IEnumerable<T> source);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="stream"></param>
        void ToStrings<T>(IEnumerable<T> source, StreamWriter stream);

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="profile"></param>
        /// <param name="source"></param>
        /// <param name="stream"></param>
        void ToStrings<T>(string profile, IEnumerable<T> source, StreamWriter stream);
    }
}
