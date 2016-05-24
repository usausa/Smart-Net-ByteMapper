namespace Smart.IO.Mapper.Mappers
{
    using System;
    using System.Text;

    /// <summary>
    ///
    /// </summary>
    internal class ConstantMapper : IFieldMapper
    {
        private readonly int offset;

        private readonly byte[] constant;

        /// <summary>
        ///
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="constant"></param>
        public ConstantMapper(int offset, byte[] constant)
        {
            this.offset = offset;
            this.constant = constant;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="buffer"></param>
        /// <param name="target"></param>
        public void FromByte(Encoding encoding, byte[] buffer, object target)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="buffer"></param>
        /// <param name="target"></param>
        public void ToByte(Encoding encoding, byte[] buffer, object target)
        {
            Array.Copy(constant, 0, buffer, offset, constant.Length);
        }
    }
}
