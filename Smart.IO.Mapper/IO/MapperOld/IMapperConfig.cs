namespace Smart.IO.MapperOld
{
    using System;
    using System.Text;

    using Smart.IO.MapperOld.Mappers;

    /// <summary>
    ///
    /// </summary>
    public interface IMapperConfig
    {
        /// <summary>
        ///
        /// </summary>
        Encoding Encoding { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        ITypeMapper FindTypeMapper(string profile, Type type);
    }
}
