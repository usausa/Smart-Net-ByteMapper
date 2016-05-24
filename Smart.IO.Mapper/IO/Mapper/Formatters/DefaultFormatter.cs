namespace Smart.IO.Mapper.Formatters
{
    using System;
    using System.Globalization;
    using System.Text;

    /// <summary>
    ///
    /// </summary>
    public class DefaultFormatter : IFormatter
    {
        private static readonly byte[] Empty = new byte[0];

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="encoding"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Framework only")]
        public byte[] Format(Type type, Encoding encoding, object value)
        {
            if (value == null)
            {
                return Empty;
            }

            return encoding.GetBytes(Convert.ToString(value, CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="encoding"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Framework only")]
        public object Parse(Type type, Encoding encoding, byte[] buffer, int offset, int length)
        {
            try
            {
                return Convert.ChangeType(encoding.GetString(buffer, offset, length), Nullable.GetUnderlyingType(type) ?? type, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                return DefaultValue.Of(type);
            }
        }
    }
}
