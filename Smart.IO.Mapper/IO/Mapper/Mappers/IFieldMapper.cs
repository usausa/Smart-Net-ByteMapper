namespace Smart.IO.Mapper.Mappers
{
    using System.Text;

    /// <summary>
    ///
    /// </summary>
    public interface IFieldMapper
    {
        /// <summary>
        ///
        /// </summary>
        int RequiredLength { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="buffer"></param>
        /// <param name="target"></param>
        void FromByte(Encoding encoding, byte[] buffer, object target);

        /// <summary>
        ///
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="buffer"></param>
        /// <param name="target"></param>
        void ToByte(Encoding encoding, byte[] buffer, object target);
    }
}
