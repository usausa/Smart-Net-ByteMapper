namespace Smart.IO.ByteMapper.Mock
{
    using System;

    public static class TestBytes
    {
        public static unsafe byte[] Offset(int offset, byte[] bytes)
        {
            var length = bytes.Length;
            var buffer = new byte[offset + length];

            fixed (byte* pSrc = &bytes[0])
            fixed (byte* pDst = &buffer[offset])
            {
                Buffer.MemoryCopy(pSrc, pDst, length, length);
            }

            return buffer;
        }
    }
}
