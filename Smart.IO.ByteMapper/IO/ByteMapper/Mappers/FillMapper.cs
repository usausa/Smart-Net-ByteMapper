namespace Smart.IO.ByteMapper.Mappers
{
    using System;

    using Smart.IO.ByteMapper.Helpers;

    public sealed class FillMapper : IMapper
    {
        private readonly int offset;

        private readonly int length;

        private readonly byte filler;

        public bool CanRead => false;

        public bool CanWrite => true;

        public FillMapper(int offset, int length, byte filler)
        {
            this.offset = offset;
            this.length = length;
            this.filler = filler;
        }

        public void Read(byte[] buffer, int index, object target)
        {
            throw new NotSupportedException();
        }

        public void Write(byte[] buffer, int index, object target)
        {
            BytesHelper.Fill(buffer, index + offset, length, filler);
        }
    }
}
