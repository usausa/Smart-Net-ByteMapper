namespace Smart.IO.Mapper.Helpers
{
    using System;
    using System.Runtime.CompilerServices;

    public static class BytesHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyPadRight(byte[] bytes, byte[] buffer, int offset, int length, byte filler)
        {
            Buffer.BlockCopy(bytes, 0, buffer, offset, bytes.Length);
            buffer.Fill(offset + bytes.Length, length - bytes.Length, filler);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyPadLeft(byte[] bytes, byte[] buffer, int offset, int length, byte filler)
        {
            Buffer.BlockCopy(bytes, 0, buffer, offset + length - bytes.Length, bytes.Length);
            buffer.Fill(offset, length - bytes.Length, filler);
        }
    }
}
