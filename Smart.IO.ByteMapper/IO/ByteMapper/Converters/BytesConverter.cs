namespace Smart.IO.ByteMapper.Converters
{
    using System;

    using Smart.IO.ByteMapper.Helpers;

    internal sealed class BytesConverter : IMapConverter
    {
        private readonly int length;

        private readonly byte filler;

        public BytesConverter(int length, byte filler)
        {
            this.length = length;
            this.filler = filler;
        }

        public unsafe object Read(byte[] buffer, int index)
        {
            var bytes = new byte[length];

            fixed (byte* pSrc = &buffer[index])
            fixed (byte* pDst = &bytes[0])
            {
                Buffer.MemoryCopy(pSrc, pDst, length, length);
            }

            return bytes;
        }

        public unsafe void Write(byte[] buffer, int index, object value)
        {
            if (value == null)
            {
                BytesHelper.Fill(buffer, index, length, filler);
            }
            else
            {
                var bytes = (byte[])value;
                BytesHelper.CopyBytes(bytes, buffer, index, length, Padding.Right, filler);
            }
        }
    }
}
