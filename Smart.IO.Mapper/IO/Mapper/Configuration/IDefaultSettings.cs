namespace Smart.IO.Mapper.Configuration
{
    using System;

    /// <summary>
    ///
    /// </summary>
    internal interface IDefaultSettings
    {
        // TODO nullvalue

            /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Padding GetPadding(Type type);

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        byte GetPaddingByte(Type type);

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool GetTrim(Type type);

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool GetNullIfEmpty(Type type);

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IValueConverter GetConverter(Type type);
    }
}
