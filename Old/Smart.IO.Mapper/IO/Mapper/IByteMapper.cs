namespace Smart.IO.Mapper
{
    /// <summary>
    ///
    /// </summary>
    public interface IByteMapper
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        T FromByte<T>(byte[] buffer)
            where T : new();
    }
}
