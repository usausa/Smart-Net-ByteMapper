namespace Smart.IO.Mapper.Mappers
{
    using System;

    public sealed class ConstMapper : IMemberMapper
    {
        private readonly int offset;

        private readonly byte[] constant;

        public int Length => constant.Length;

        public bool CanRead => false;

        public bool CanWrite => true;

        public ConstMapper(int offset, byte[] constant)
        {
            this.offset = offset;
            this.constant = constant;
        }

        public void Read(byte[] buffer, int index, object target)
        {
        }

        public void Write(byte[] buffer, int index, object target)
        {
            Buffer.BlockCopy(constant, 0, buffer, index + offset, constant.Length);
        }
    }
}
