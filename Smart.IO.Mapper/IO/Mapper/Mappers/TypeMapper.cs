namespace Smart.IO.Mapper.Mappers
{
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    ///
    /// </summary>
    internal class TypeMapper : ITypeMapper
    {
        private readonly int length;

        private readonly byte filler;

        private readonly IList<IFieldMapper> fields = new List<IFieldMapper>();

        /// <summary>
        ///
        /// </summary>
        /// <param name="length"></param>
        /// <param name="filler"></param>
        public TypeMapper(int length, byte filler)
        {
            this.length = length;
            this.filler = filler;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fieldMapper"></param>
        internal void AddFiled(IFieldMapper fieldMapper)
        {
            fields.Add(fieldMapper);
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
            var buffer = new byte[length];
            if (filler != 0)
            {
                buffer.Fill(0, length, filler);
            }

            foreach (var field in fields)
            {
                field.ToByte(encoding, buffer, target);
            }

            return buffer;
        }
    }
}
