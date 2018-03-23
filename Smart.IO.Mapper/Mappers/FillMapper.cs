namespace Smart.IO.Mapper.Mappers
{
    public sealed class FillMapper : IMemberMapper
    {
        private readonly int offset;

        private readonly byte filler;

        public int Length { get; }

        public FillMapper(int offset, int length, byte filler)
        {
            this.offset = offset;
            Length = length;
            this.filler = filler;
        }

        public void Read(byte[] buffer, int index, object target)
        {
        }

        public void Write(byte[] buffer, int index, object target)
        {
            buffer.Fill(index + offset, Length, filler);
        }
    }
}
