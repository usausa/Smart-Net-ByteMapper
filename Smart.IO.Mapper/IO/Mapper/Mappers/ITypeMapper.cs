namespace Smart.IO.Mapper.Mappers
{
    using System.Text;

    /// <summary>
    ///
    /// </summary>
    public interface ITypeMapper
    {
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
        /// <param name="target"></param>
        /// <returns></returns>
        byte[] ToByte(Encoding encoding, object target);
    }
}
