namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Smart.IO.Mapper.Mappers;

    /// <summary>
    ///
    /// </summary>
    internal class MapperConfig : IMapperConfig
    {
        private readonly Dictionary<Type, ITypeMapper> typeEntries = new Dictionary<Type, ITypeMapper>();

        /// <summary>
        ///
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.Default;

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ITypeMapper FindTypeMapper(Type type)
        {
            ITypeMapper typeMapper;
            return typeEntries.TryGetValue(type, out typeMapper) ? typeMapper : null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typeMapper"></param>
        public void AddTypeMapper(Type type, ITypeMapper typeMapper)
        {
            typeEntries[type] = typeMapper;
        }
    }
}
