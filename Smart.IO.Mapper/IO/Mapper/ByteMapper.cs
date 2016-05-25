namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;

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
        /// <param name="buffer"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public T FromByte<T>(byte[] buffer, T target)
        {
            var type = typeof(T);
            var mapper = FindTypeMapper(type);

            mapper.FromByte(mapperConfig.Encoding, buffer, target);

            return target;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public IEnumerable<T> FromByte<T>(Stream stream)
            where T : new()
        {
            var type = typeof(T);
            var mapper = FindTypeMapper(type);

            var buffer = new byte[mapper.Length];
            while (stream.Read(buffer, 0, buffer.Length) == buffer.Length)
            {
                var obj = new T();
                mapper.FromByte(mapperConfig.Encoding, buffer, obj);
                yield return obj;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public IEnumerable<T> FromByte<T>(Stream stream, Func<T> factory)
        {
            var type = typeof(T);
            var mapper = FindTypeMapper(type);

            var buffer = new byte[mapper.Length];
            while (stream.Read(buffer, 0, buffer.Length) == buffer.Length)
            {
                var obj = factory();
                mapper.FromByte(mapperConfig.Encoding, buffer, obj);
                yield return obj;
            }
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

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="stream"></param>
        public void ToByte<T>(IEnumerable<T> source, Stream stream)
        {
            var type = typeof(T);
            var mapper = FindTypeMapper(type);

            foreach (var obj in source)
            {
                var buffer = mapper.ToByte(mapperConfig.Encoding, obj);
                stream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
