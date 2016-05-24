namespace Smart.IO.Mapper.Mappers
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
        internal IFormatter Formatter { get; set; }

        /// <summary>
        ///
        /// </summary>
        internal IAccessor Accessor { get; set; }

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

            var start = offset;
            var end = offset + length;
            if (Trim)
            {
                while ((start < end) && (buffer[start] == PaddingByte))
                {
                    start++;
                }

                while ((start < end) && (buffer[end - 1] == PaddingByte))
                {
                    end--;
                }
            }

            var value = (start >= end) && NullIfEmpty
                ? DefaultValue.Of(Accessor.Type)
                : Formatter.Parse(Accessor.Type, encoding, buffer, start, end - start);

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
            if ((value == null) && NullIfEmpty)
            {
                buffer.Fill(offset, length, PaddingByte);
            }
            else
            {
                var bytes = Formatter.Format(Accessor.Type, encoding, value);
                if (bytes.Length >= length)
                {
                    Array.Copy(bytes, 0, buffer, offset, length);
                }
                else
                {
                    Array.Copy(bytes, 0, buffer, Padding == Padding.Right ? offset : offset + length - bytes.Length, bytes.Length);
                    buffer.Fill(Padding == Padding.Right ? offset + bytes.Length : offset, length - bytes.Length, PaddingByte);
                }
            }
        }
    }
}
