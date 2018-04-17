namespace Smart.IO.ByteMapper.Helpers
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;

    public static class BytesHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Fill(byte[] byets, int offset, int length, byte value)
        {
            for (var i = 0; i < length; i++)
            {
                byets[offset + i] = value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyPadRight(byte[] bytes, byte[] buffer, int offset, int length, byte filler)
        {
            Buffer.BlockCopy(bytes, 0, buffer, offset, bytes.Length);
            Fill(buffer, offset + bytes.Length, length - bytes.Length, filler);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyPadLeft(byte[] bytes, byte[] buffer, int offset, int length, byte filler)
        {
            Buffer.BlockCopy(bytes, 0, buffer, offset + length - bytes.Length, bytes.Length);
            Fill(buffer, offset, length - bytes.Length, filler);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadString(byte[] buffer, int offset, int length, Encoding encoding, bool trim, Padding padding, byte filler)
        {
            var start = offset;
            var size = length;
            if (trim)
            {
                if (padding == Padding.Left)
                {
                    var end = start + length;
                    while ((start < end) && (buffer[start] == filler))
                    {
                        start++;
                        size--;
                    }
                }
                else
                {
                    while ((size > 0) && (buffer[start + size - 1] == filler))
                    {
                        size--;
                    }
                }
            }

            // TODO range取得にする？
            return encoding.GetString(buffer, start, size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteString(string value, byte[] buffer, int offset, int length, Encoding encoding, Padding padding, byte filler)
        {
            // TODO bytes外？ readとの対応！
            var bytes = encoding.GetBytes(value);
            if (bytes.Length >= length)
            {
                Buffer.BlockCopy(bytes, 0, buffer, offset, length);
            }
            else if (padding == Padding.Right)
            {
                CopyPadRight(bytes, buffer, offset, length, filler);
            }
            else
            {
                CopyPadLeft(bytes, buffer, offset, length, filler);
            }
        }

        public static Type GetConvertEnumType(Type type)
        {
            var targetType = type.IsNullableType() ? Nullable.GetUnderlyingType(type) : type;
            return (targetType?.IsEnum ?? false) ? targetType : null;
        }
    }
}
