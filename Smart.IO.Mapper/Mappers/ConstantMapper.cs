namespace Smart.IO.Mapper.Mappers
{
    using System;

    public sealed class ConstantMapper : IMapper
    {
        private readonly int offset;

        private readonly byte[] content;

        public bool CanRead => false;

        public bool CanWrite => true;

        public ConstantMapper(int offset, byte[] content)
        {
            this.offset = offset;
            this.content = content;
        }

        public void Read(byte[] buffer, int index, object target)
        {
            throw new NotSupportedException();
        }

        public void Write(byte[] buffer, int index, object target)
        {
            Buffer.BlockCopy(content, 0, buffer, index + offset, content.Length);
        }
    }
}
