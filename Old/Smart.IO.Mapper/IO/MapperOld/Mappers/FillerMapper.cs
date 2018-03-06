namespace Smart.IO.MapperOld.Mappers
{
    using System.Text;

    /// <summary>
    ///
    /// </summary>
    internal class FillerMapper : IFieldMapper
    {
        private readonly int offset;

        private readonly int length;

        private readonly byte filler;

        /// <summary>
        ///
        /// </summary>
        public int RequiredLength => 0;

        /// <summary>
        ///
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="filler"></param>
        internal FillerMapper(int offset, int length, byte filler)
        {
            this.offset = offset;
            this.length = length;
            this.filler = filler;
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
            buffer.Fill(offset, length, filler);
        }
    }
}
