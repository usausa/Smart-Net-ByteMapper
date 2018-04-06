namespace Smart.IO.Mapper.Converters
{
    using System;

    using Smart.IO.Mapper.Helpers;

    public sealed class MapsConverter : IMapConverter
    {
        private readonly int length;

        private readonly byte filler;

        public MapsConverter(int length, byte filler)
        {
            this.length = length;
            this.filler = filler;
        }

        public object Read(byte[] buffer, int index)
        {
            var bytes = new byte[length];
            Buffer.BlockCopy(buffer, index, bytes, 0, length);
            return bytes;
        }

        public void Write(byte[] buffer, int index, object value)
        {
            if (value == null)
            {
                buffer.Fill(index, length, filler);
            }
            else
            {
                var bytes = (byte[])value;
                if (bytes.Length >= length)
                {
                    Buffer.BlockCopy(bytes, 0, buffer, index, length);
                }
                else
                {
                    BytesHelper.CopyPadRight(bytes, buffer, index, length, filler);
                }
            }
        }
    }
}
