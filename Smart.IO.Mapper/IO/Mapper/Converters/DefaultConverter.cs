namespace Smart.IO.Mapper.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    /// <summary>
    ///
    /// </summary>
    public class DefaultConverter : IValueConverter
    {
        private static readonly byte[] Empty = new byte[0];

        private static readonly Dictionary<Type, Func<string, NumberStyles, IFormatProvider, object>> Parsers =
            new Dictionary<Type, Func<string, NumberStyles, IFormatProvider, object>>
        {
            { typeof(byte), (str, style, provider) => byte.Parse(str, style, provider) },
            { typeof(short), (str, style, provider) => short.Parse(str, style, provider) },
            { typeof(int), (str, style, provider) => int.Parse(str, style, provider) },
            { typeof(long), (str, style, provider) => long.Parse(str, style, provider) },
            { typeof(sbyte), (str, style, provider) => sbyte.Parse(str, style, provider) },
            { typeof(ushort), (str, style, provider) => ushort.Parse(str, style, provider) },
            { typeof(uint), (str, style, provider) => uint.Parse(str, style, provider) },
            { typeof(ulong), (str, style, provider) => ulong.Parse(str, style, provider) }
        };

        private readonly string format;

        private readonly IFormatProvider provider;

        private readonly NumberStyles style;

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
            : this(format, null, NumberStyles.Any)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        public DefaultConverter(string format, IFormatProvider provider)
            : this(format, provider, NumberStyles.Any)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="format"></param>
        /// <param name="style"></param>
        public DefaultConverter(string format, NumberStyles style)
            : this(format, null, style)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <param name="style"></param>
        public DefaultConverter(string format, IFormatProvider provider, NumberStyles style)
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
            var str = encoding.GetString(buffer, offset, length);
            var valueType = Nullable.GetUnderlyingType(type);
            var targetType = valueType ?? type;

            try
            {
                Func<string, NumberStyles, IFormatProvider, object> parser;
                if (Parsers.TryGetValue(targetType, out parser))
                {
                    return parser(str, style, provider);
                }

                return Convert.ChangeType(str, targetType, provider);
            }
            catch (FormatException)
            {
                return valueType == null ? (object)default(DateTime) : null;
            }
        }
    }
}
