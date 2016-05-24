namespace Smart.IO.Mapper
{
    using System;
    using System.Globalization;

    using Smart.IO.Mapper.Mappers;

    /// <summary>
    ///
    /// </summary>
    public class ByteMapper : IByteMapper
    {
        private readonly IMapperConfig mapperConfig;

        /// <summary>
        ///
        /// </summary>
        /// <param name="mapperConfig"></param>
        public ByteMapper(IMapperConfig mapperConfig)
        {
            this.mapperConfig = mapperConfig;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private ITypeMapper FindTypeMapper(Type type)
        {
            var mapper = mapperConfig.FindTypeMapper(type);
            if (mapper == null)
            {
                throw new ByteMapperException(String.Format(CultureInfo.InvariantCulture, "Type {0} is not configured.", type));
            }

            return mapper;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public T FromByte<T>(byte[] buffer)
            where T : new()
        {
            var type = typeof(T);
            var mapper = FindTypeMapper(type);

            var obj = new T();
            mapper.FromByte(mapperConfig.Encoding, buffer, obj);

            return obj;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public byte[] ToByte<T>(T source)
        {
            var type = typeof(T);
            var mapper = FindTypeMapper(type);

            return mapper.ToByte(mapperConfig.Encoding, source);
        }
    }
}
