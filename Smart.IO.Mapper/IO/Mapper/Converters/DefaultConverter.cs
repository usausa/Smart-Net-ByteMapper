namespace Smart.IO.Mapper.Converters
{
    using System;
    using System.Text;

    /// <summary>
    ///
    /// </summary>
    public class DefaultConverter : IValueConverter
    {
        private static readonly byte[] Empty = new byte[0];

        private readonly string format;

        private readonly IFormatProvider provider;

        /// <summary>
        ///
        /// </summary>
        public DefaultConverter()
            : this(null, null)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="format"></param>
        public DefaultConverter(string format)
            : this(format, null)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        public DefaultConverter(string format, IFormatProvider provider)
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Framework only")]
        public byte[] ToByte(Type type, Encoding encoding, object value)
        {
            if (value == null)
            {
                return Empty;
            }

            if (!String.IsNullOrEmpty(format))
            {
                var formatable = value as IFormattable;
                if (formatable != null)
                {
                    return encoding.GetBytes(formatable.ToString(format, provider));
                }
            }

            return encoding.GetBytes(Convert.ToString(value, provider));
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
                return Convert.ChangeType(encoding.GetString(buffer, offset, length), Nullable.GetUnderlyingType(type) ?? type, provider);
            }
            catch (FormatException)
            {
                return Nullable.GetUnderlyingType(type) == null ? (object)default(DateTime) : null;
            }
        }
    }
}
