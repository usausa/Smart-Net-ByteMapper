namespace Smart.IO.Mapper.Converters
{
    using System;
    using System.Text;

    public class BooleanConverter : IValueConverter
    {
        private static readonly byte[] Empty = new byte[0];

        private readonly byte[] trueValue;

        private readonly byte[] falseValue;

        /// <summary>
        ///
        /// </summary>
        /// <param name="trueValue"></param>
        /// <param name="falseValue"></param>
        public BooleanConverter(byte[] trueValue, byte[] falseValue)
        {
            this.trueValue = trueValue;
            this.falseValue = falseValue;
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
            return !falseValue.ArrayEquals(0, buffer, offset, length);
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

            return (bool)value ? trueValue : falseValue;
        }
    }
}
