namespace Smart.IO.MapperOld.Mappers
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
        int Length { get; }

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
        /// <param name="appendDelimiter"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        byte[] ToByte(Encoding encoding, bool appendDelimiter, object target);
    }
}
