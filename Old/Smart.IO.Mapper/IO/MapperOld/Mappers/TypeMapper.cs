namespace Smart.IO.MapperOld.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    ///
    /// </summary>
    internal class TypeMapper : ITypeMapper
    {
        private readonly byte filler;

        private readonly byte[] delimiter;

        private readonly IList<IFieldMapper> fields = new List<IFieldMapper>();

        /// <summary>
        ///
        /// </summary>
        public int Length { get; }

        /// <summary>
        ///
        /// </summary>
        public int RequiredLength { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="length"></param>
        /// <param name="filler"></param>
        /// <param name="delimiter"></param>
        public TypeMapper(int length, byte filler, byte[] delimiter)
        {
            Length = length;
            this.filler = filler;
            this.delimiter = delimiter;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fieldMapper"></param>
        internal void AddFiled(IFieldMapper fieldMapper)
        {
            fields.Add(fieldMapper);
            RequiredLength = Math.Max(RequiredLength, fieldMapper.RequiredLength);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="buffer"></param>
        /// <param name="target"></param>
        public void FromByte(Encoding encoding, byte[] buffer, object target)
        {
            foreach (var field in fields)
            {
                field.FromByte(encoding, buffer, target);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="appendDelimiter"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public byte[] ToByte(Encoding encoding, bool appendDelimiter, object target)
        {
            var buffer = new byte[Length - (appendDelimiter ? 0 : delimiter.Length)];
            if (filler != 0)
            {
                buffer.Fill(0, Length - (appendDelimiter ? delimiter.Length : 0), filler);
            }

            foreach (var field in fields)
            {
                field.ToByte(encoding, buffer, target);
            }

            if (appendDelimiter)
            {
                Buffer.BlockCopy(delimiter, 0, buffer, Length - delimiter.Length, delimiter.Length);
            }

            return buffer;
        }
    }
}
