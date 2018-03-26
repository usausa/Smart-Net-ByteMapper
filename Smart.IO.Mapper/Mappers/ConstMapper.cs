namespace Smart.IO.Mapper.Mappers
{
    using System;

    public sealed class ConstMapper : IMemberMapper
    {
        private readonly int offset;

        private readonly byte[] content;

        public int Length => content.Length;

        public bool CanRead => false;

        public bool CanWrite => true;

        public ConstMapper(int offset, byte[] content)
        {
            this.offset = offset;
            this.content = content;
        }

        public void Read(byte[] buffer, int index, object target)
        {
        }

        public void Write(byte[] buffer, int index, object target)
        {
            Buffer.BlockCopy(content, 0, buffer, index + offset, content.Length);
        }
    }
}
