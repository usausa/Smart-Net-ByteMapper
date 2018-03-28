namespace Smart.IO.Mapper.Converters
{
    using System;

    using Smart.IO.Mapper.Helpers;

    public sealed class BytesConverter : IByteConverter
    {
        private readonly byte filler;

        public int Length { get; }

        public BytesConverter(int length, byte filler)
        {
            Length = length;
            this.filler = filler;
        }

        public object Read(byte[] buffer, int index)
        {
            var bytes = new byte[Length];
            Buffer.BlockCopy(buffer, index, bytes, 0, Length);
            return bytes;
        }

        public void Write(byte[] buffer, int index, object value)
        {
            if (value == null)
            {
                buffer.Fill(index, Length, filler);
            }
            else
            {
                var bytes = (byte[])value;
                if (bytes.Length >= Length)
                {
                    Buffer.BlockCopy(bytes, 0, buffer, index, Length);
                }
                else
                {
                    BytesHelper.CopyPadRight(bytes, buffer, index, Length, filler);
                }
            }
        }
    }
}
