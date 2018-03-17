namespace Smart.IO.Mapper.Mappers
{
    public sealed class FillMapper : IMemberMapper
    {
        private readonly int offset;

        private readonly int length;

        private readonly byte filler;

        public FillMapper(int offset, int length, byte filler)
        {
            this.offset = offset;
            this.length = length;
            this.filler = filler;
        }

        public void Read(byte[] buffer, object target)
        {
        }

        public void Write(byte[] buffer, object target)
        {
            buffer.Fill(offset, length, filler);
        }
    }
}
