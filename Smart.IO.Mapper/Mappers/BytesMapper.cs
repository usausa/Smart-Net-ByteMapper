namespace Smart.IO.Mapper.Mappers
{
    using System;

    using Smart.IO.Mapper.Helpers;

    public sealed class BytesMapper : IMemberMapper
    {
        private readonly int offset;

        private readonly int length;

        private readonly Func<object, object> getter;

        private readonly Action<object, object> setter;

        private readonly byte filler;

        public BytesMapper(
            int offset,
            int length,
            Func<object, object> getter,
            Action<object, object> setter,
            byte filler)
        {
            this.offset = offset;
            this.length = length;
            this.getter = getter;
            this.setter = setter;
            this.filler = filler;
        }

        public void Read(byte[] buffer, int index, object target)
        {
            var bytes = new byte[length];
            Buffer.BlockCopy(buffer, index + offset, bytes, 0, length);
            setter(target, bytes);
        }

        public void Write(byte[] buffer, int index, object target)
        {
            var bytes = (byte[])getter(target);
            if (bytes == null)
            {
                buffer.Fill(index + offset, length, filler);
            }
            else
            {
                if (bytes.Length >= length)
                {
                    Buffer.BlockCopy(bytes, 0, buffer, index + offset, length);
                }
                else
                {
                    BytesHelper.CopyPadRight(bytes, buffer, index + offset, length, filler);
                }
            }
        }
    }
}
