namespace Smart.IO.Mapper.Mappings
{
    using System;

    public sealed class ConstMapping : IMapping
    {
        private readonly int offset;

        private readonly byte[] content;

        public bool CanRead => false;

        public bool CanWrite => true;

        public ConstMapping(int offset, byte[] content)
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
