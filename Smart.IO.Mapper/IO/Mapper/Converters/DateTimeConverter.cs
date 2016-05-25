namespace Smart.IO.Mapper.Converters
{
    using System;
    using System.Globalization;
    using System.Text;

    /// <summary>
    ///
    /// </summary>
    public class DateTimeConverter : IValueConverter
    {
        private static readonly byte[] Empty = new byte[0];

        private readonly string format;

        private readonly IFormatProvider provider;

        private readonly DateTimeStyles style;

        /// <summary>
        ///
        /// </summary>
        /// <param name="format"></param>
        public DateTimeConverter(string format)
            : this(format, null, DateTimeStyles.None)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        public DateTimeConverter(string format, IFormatProvider provider)
            : this(format, provider, DateTimeStyles.None)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <param name="style"></param>
        public DateTimeConverter(string format, IFormatProvider provider, DateTimeStyles style)
        {
            this.format = format;
            this.provider = provider;
            this.style = style;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="encoding"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Framework only")]
        public byte[] ToByte(Type type, Encoding encoding, object value)
        {
            if (value == null)
            {
                return Empty;
            }

            return encoding.GetBytes(((DateTime)value).ToString(format, provider));
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
        public object FromByte(Type type, Encoding encoding, byte[] buffer, int offset, int length)
        {
            try
            {
                return DateTime.ParseExact(encoding.GetString(buffer, offset, length), format, provider, style);
            }
            catch (FormatException)
            {
                return DefaultValue.Of(type);
            }
        }
    }
}
