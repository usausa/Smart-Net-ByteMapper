namespace Smart.IO.Mapper.Mock
{
    using System;

    public static class TestBytes
    {
        public static byte[] Offset(int offset, byte[] bytes)
        {
            var buffer = new byte[offset + bytes.Length];
            Buffer.BlockCopy(bytes, 0, buffer, offset, bytes.Length);
            return buffer;
        }
    }
}
