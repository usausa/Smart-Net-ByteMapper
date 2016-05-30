namespace Smart.IO.Mapper.Mappers
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
        public TypeMapper(int length, byte filler)
        {
            Length = length;
            this.filler = filler;
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
        /// <param name="target"></param>
        /// <returns></returns>
        public byte[] ToByte(Encoding encoding, object target)
        {
            var buffer = new byte[Length];
            if (filler != 0)
            {
                buffer.Fill(0, Length, filler);
            }

            foreach (var field in fields)
            {
                field.ToByte(encoding, buffer, target);
            }

            return buffer;
        }
    }
}
