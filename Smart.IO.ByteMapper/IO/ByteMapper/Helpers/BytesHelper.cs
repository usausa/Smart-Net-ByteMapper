namespace Smart.IO.ByteMapper.Helpers
{
    using System;
    using System.Runtime.CompilerServices;

    public static class BytesHelper
    {
        //--------------------------------------------------------------------------------
        // Enum
        //--------------------------------------------------------------------------------

        public static Type GetConvertEnumType(Type type)
        {
            var targetType = type.IsNullableType() ? Nullable.GetUnderlyingType(type) : type;
            return (targetType?.IsEnum ?? false) ? targetType : null;
        }

        //--------------------------------------------------------------------------------
        // Fill
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Fill(byte[] byets, int index, int length, byte value)
        {
            for (var i = 0; i < length; i++)
            {
                byets[index + i] = value;
            }
        }

        //--------------------------------------------------------------------------------
        // Copy
        //--------------------------------------------------------------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyPadRight(byte[] bytes, byte[] buffer, int index, int length, byte filler)
        {
            Buffer.BlockCopy(bytes, 0, buffer, index, bytes.Length);
            Fill(buffer, index + bytes.Length, length - bytes.Length, filler);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyPadLeft(byte[] bytes, byte[] buffer, int index, int length, byte filler)
        {
            Buffer.BlockCopy(bytes, 0, buffer, index + length - bytes.Length, bytes.Length);
            Fill(buffer, index, length - bytes.Length, filler);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TrimRange(byte[] buffer, ref int start, ref int size, Padding padding, byte filler)
        {
            if (padding == Padding.Left)
            {
                var end = start + size;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBytes(byte[] bytes, byte[] buffer, int index, int length, Padding padding, byte filler)
        {
            if (bytes.Length >= length)
            {
                Buffer.BlockCopy(bytes, 0, buffer, index, length);
            }
            else if (padding == Padding.Right)
            {
                CopyPadRight(bytes, buffer, index, length, filler);
            }
            else
            {
                CopyPadLeft(bytes, buffer, index, length, filler);
            }
        }
    }
}
