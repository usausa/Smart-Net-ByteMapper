namespace Smart.IO.Mapper.Converters
{
    using System;
    using System.Text;

    /// <summary>
    ///
    /// </summary>
    public class DateTimeConverter : IValueConverter
    {
        private static readonly byte[] Empty = new byte[0];

        private readonly string format;

        private readonly IFormatProvider provider;

        /// <summary>
        ///
        /// </summary>
        /// <param name="format"></param>
        public DateTimeConverter(string format)
            : this(format, null)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        public DateTimeConverter(string format, IFormatProvider provider)
        {
            this.format = format;
            this.provider = provider;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="encoding"></param>
        /// <param name="value"></param>
        /// <returns></returns>
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
        public object FromByte(Type type, Encoding encoding, byte[] buffer, int offset, int length)
        {
            try
            {
                return DateTime.ParseExact(encoding.GetString(buffer, offset, length), format, provider);
            }
            catch (FormatException)
            {
                return DefaultValue.Of(type);
            }
        }
    }
}
