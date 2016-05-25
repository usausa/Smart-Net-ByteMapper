namespace Smart.IO.Mapper
{
    /// <summary>
    ///
    /// </summary>
    public interface IMemberConfigurationExpression
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="filler"></param>
        /// <returns></returns>
        IMemberConfigurationExpression Padding(byte filler);

        /// <summary>
        ///
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        IMemberConfigurationExpression Padding(Padding direction);

        /// <summary>
        ///
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="filler"></param>
        /// <returns></returns>
        IMemberConfigurationExpression Padding(Padding direction, byte filler);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IMemberConfigurationExpression Trim(bool value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IMemberConfigurationExpression NullIfEmpty(bool value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IMemberConfigurationExpression NullValue(byte[] value);

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IMemberConfigurationExpression Converter(IValueConverter value);
    }
}
