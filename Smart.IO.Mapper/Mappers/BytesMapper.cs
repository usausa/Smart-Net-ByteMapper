namespace Smart.IO.Mapper.Mappers
{
    using System;

    using Smart.IO.Mapper.Helpers;

    public sealed class BytesMapper : IMemberMapper
    {
        private readonly int offset;

        private readonly Func<object, object> getter;

        private readonly Action<object, object> setter;

        private readonly byte filler;

        public int Length { get; }

        public BytesMapper(
            int offset,
            int length,
            Func<object, object> getter,
            Action<object, object> setter,
            byte filler)
        {
            this.offset = offset;
            Length = length;
            this.getter = getter;
            this.setter = setter;
            this.filler = filler;
        }

        public void Read(byte[] buffer, int index, object target)
        {
            var bytes = new byte[Length];
            Buffer.BlockCopy(buffer, index + offset, bytes, 0, Length);
            setter(target, bytes);
        }

        public void Write(byte[] buffer, int index, object target)
        {
            var bytes = (byte[])getter(target);
            if (bytes == null)
            {
                buffer.Fill(index + offset, Length, filler);
            }
            else
            {
                if (bytes.Length >= Length)
                {
                    Buffer.BlockCopy(bytes, 0, buffer, index + offset, Length);
                }
                else
                {
                    BytesHelper.CopyPadRight(bytes, buffer, index + offset, Length, filler);
                }
            }
        }
    }
}
