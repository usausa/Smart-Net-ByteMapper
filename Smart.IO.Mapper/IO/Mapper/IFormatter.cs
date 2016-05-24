namespace Smart.IO.Mapper
{
    using System;
    using System.Text;

    /// <summary>
    ///
    /// </summary>
    public interface IFormatter
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="encoding"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        byte[] Format(Type type, Encoding encoding, object value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="encoding"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        object Parse(Type type, Encoding encoding, byte[] buffer, int offset, int length);
    }
}
