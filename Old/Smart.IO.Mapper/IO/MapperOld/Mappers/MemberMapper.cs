namespace Smart.IO.MapperOld.Mappers
{
    using System;
    using System.Text;

    using Smart.Reflection;

    /// <summary>
    ///
    /// </summary>
    internal class MemberMapper : IFieldMapper
    {
        private readonly int offset;

        private readonly int length;

        /// <summary>
        ///
        /// </summary>
        internal Padding Padding { get; set; }

        /// <summary>
        ///
        /// </summary>
        internal byte PaddingByte { get; set; }

        /// <summary>
        ///
        /// </summary>
        internal bool Trim { get; set; }

        /// <summary>
        ///
        /// </summary>
        internal bool NullIfEmpty { get; set; }

        /// <summary>
        ///
        /// </summary>
        internal byte[] NullValue { get; set; }

        /// <summary>
        ///
        /// </summary>
        internal IValueConverter Converter { get; set; }

        /// <summary>
        ///
        /// </summary>
        internal IAccessor Accessor { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int RequiredLength => offset + length;

        /// <summary>
        ///
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        internal MemberMapper(int offset, int length)
        {
            this.offset = offset;
            this.length = length;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="buffer"></param>
        /// <param name="target"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Framework only")]
        public void FromByte(Encoding encoding, byte[] buffer, object target)
        {
            if (!Accessor.CanWrite)
            {
                return;
            }

            var isNullValue = (NullValue != null) && NullValue.ArrayEquals(0, buffer, offset, length);

            var start = offset;
            var size = length;
            if (!isNullValue && Trim)
            {
                if (Padding == Padding.Left)
                {
                    while ((start < offset + length) && (buffer[start] == PaddingByte))
                    {
                        start++;
                        size--;
                    }
                }
                else
                {
                    while ((size > 0) && (buffer[offset + size - 1] == PaddingByte))
                    {
                        size--;
                    }
                }
            }

            var value = isNullValue || ((size <= 0) && NullIfEmpty)
                ? Accessor.Type.GetDefaultValue()
                : Converter.FromByte(Accessor.Type, encoding, buffer, start, size);

            Accessor.SetValue(target, value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="buffer"></param>
        /// <param name="target"></param>
        public void ToByte(Encoding encoding, byte[] buffer, object target)
        {
            if (!Accessor.CanRead)
            {
                return;
            }

            var value = Accessor.GetValue(target);
            if ((value == null) && (NullValue != null))
            {
                Buffer.BlockCopy(NullValue, 0, buffer, offset, Math.Min(NullValue.Length, length));
            }
            else if ((value == null) && NullIfEmpty)
            {
                buffer.Fill(offset, length, PaddingByte);
            }
            else
            {
                var bytes = Converter.ToByte(Accessor.Type, encoding, value);
                if (bytes.Length >= length)
                {
                    Buffer.BlockCopy(bytes, Padding == Padding.Right ? 0 : bytes.Length - length, buffer, offset, length);
                }
                else
                {
                    Buffer.BlockCopy(bytes, 0, buffer, Padding == Padding.Right ? offset : offset + length - bytes.Length, bytes.Length);
                    buffer.Fill(Padding == Padding.Right ? offset + bytes.Length : offset, length - bytes.Length, PaddingByte);
                }
            }
        }
    }
}
