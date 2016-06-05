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
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            var type = typeof(T);
            var mapper = FindTypeMapper(type);

            if (buffer.Length < mapper.RequiredLength)
            {
                return default(T);
            }

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
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var type = typeof(T);
            var mapper = FindTypeMapper(type);

            if (buffer.Length < mapper.RequiredLength)
            {
                return default(T);
            }

            mapper.FromByte(mapperConfig.Encoding, buffer, target);

            return target;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEnumerable<T> FromBytes<T>(IEnumerable<byte[]> source)
            where T : new()
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var type = typeof(T);
            var mapper = FindTypeMapper(type);

            foreach (var buffer in source)
            {
                if (buffer.Length < mapper.RequiredLength)
                {
                    continue;
                }

                var obj = new T();
                mapper.FromByte(mapperConfig.Encoding, buffer, obj);
                yield return obj;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public IEnumerable<T> FromBytes<T>(IEnumerable<byte[]> source, Func<T> factory)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            var type = typeof(T);
            var mapper = FindTypeMapper(type);

            foreach (var buffer in source)
            {
                if (buffer.Length < mapper.RequiredLength)
                {
                    continue;
                }

                var obj = factory();
                mapper.FromByte(mapperConfig.Encoding, buffer, obj);
                yield return obj;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public IEnumerable<T> FromBytes<T>(Stream stream)
            where T : new()
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var type = typeof(T);
            var mapper = FindTypeMapper(type);

            var buffer = new byte[mapper.Length];
            while (stream.Read(buffer, 0, buffer.Length) == buffer.Length)
            {
                if (buffer.Length < mapper.RequiredLength)
                {
                    continue;
                }

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
        public IEnumerable<T> FromBytes<T>(Stream stream, Func<T> factory)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            var type = typeof(T);
            var mapper = FindTypeMapper(type);

            var buffer = new byte[mapper.Length];
            while (stream.Read(buffer, 0, buffer.Length) == buffer.Length)
            {
                if (buffer.Length < mapper.RequiredLength)
                {
                    continue;
                }

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
            return ToByte(source, true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="appendDelimiter"></param>
        /// <returns></returns>
        public byte[] ToByte<T>(T source, bool appendDelimiter)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var type = typeof(T);
            var mapper = FindTypeMapper(type);

            return mapper.ToByte(mapperConfig.Encoding, appendDelimiter, source);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEnumerable<byte[]> ToBytes<T>(IEnumerable<T> source)
        {
            return ToBytes(source, true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="appendDelimiter"></param>
        /// <returns></returns>
        public IEnumerable<byte[]> ToBytes<T>(IEnumerable<T> source, bool appendDelimiter)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var type = typeof(T);
            var mapper = FindTypeMapper(type);

            foreach (var obj in source)
            {
                yield return mapper.ToByte(mapperConfig.Encoding, appendDelimiter, obj);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="stream"></param>
        public void ToBytes<T>(IEnumerable<T> source, Stream stream)
        {
            ToBytes(source, true, stream);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="appendDelimiter"></param>
        /// <param name="stream"></param>
        public void ToBytes<T>(IEnumerable<T> source, bool appendDelimiter, Stream stream)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var type = typeof(T);
            var mapper = FindTypeMapper(type);

            foreach (var obj in source)
            {
                var buffer = mapper.ToByte(mapperConfig.Encoding, appendDelimiter, obj);
                stream.Write(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public T FromString<T>(string str)
            where T : new()
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            var type = typeof(T);
            var mapper = FindTypeMapper(type);

            var buffer = mapperConfig.Encoding.GetBytes(str);
            if (buffer.Length < mapper.RequiredLength)
            {
                return default(T);
            }

            var obj = new T();
            mapper.FromByte(mapperConfig.Encoding, buffer, obj);

            return obj;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public T FromString<T>(string str, T target)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var type = typeof(T);
            var mapper = FindTypeMapper(type);

            var buffer = mapperConfig.Encoding.GetBytes(str);
            if (buffer.Length < mapper.RequiredLength)
            {
                return default(T);
            }

            mapper.FromByte(mapperConfig.Encoding, buffer, target);

            return target;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEnumerable<T> FromStrings<T>(IEnumerable<string> source)
            where T : new()
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var type = typeof(T);
            var mapper = FindTypeMapper(type);

            foreach (var str in source)
            {
                var buffer = mapperConfig.Encoding.GetBytes(str);
                if (buffer.Length < mapper.RequiredLength)
                {
                    continue;
                }

                var obj = new T();
                mapper.FromByte(mapperConfig.Encoding, buffer, obj);
                yield return obj;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public IEnumerable<T> FromStrings<T>(IEnumerable<string> source, Func<T> factory)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            var type = typeof(T);
            var mapper = FindTypeMapper(type);

            foreach (var str in source)
            {
                var buffer = mapperConfig.Encoding.GetBytes(str);
                if (buffer.Length < mapper.RequiredLength)
                {
                    continue;
                }

                var obj = factory();
                mapper.FromByte(mapperConfig.Encoding, buffer, obj);
                yield return obj;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public IEnumerable<T> FromStrings<T>(StreamReader stream)
            where T : new()
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var type = typeof(T);
            var mapper = FindTypeMapper(type);

            string str;
            while ((str = stream.ReadLine()) != null)
            {
                var buffer = mapperConfig.Encoding.GetBytes(str);
                if (buffer.Length < mapper.RequiredLength)
                {
                    continue;
                }

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
        public IEnumerable<T> FromStrings<T>(StreamReader stream, Func<T> factory)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            var type = typeof(T);
            var mapper = FindTypeMapper(type);

            string str;
            while ((str = stream.ReadLine()) != null)
            {
                var buffer = mapperConfig.Encoding.GetBytes(str);
                if (buffer.Length < mapper.RequiredLength)
                {
                    continue;
                }

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
        public string ToString<T>(T source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var type = typeof(T);
            var mapper = FindTypeMapper(type);

            var buffer = mapper.ToByte(mapperConfig.Encoding, false, source);

            return mapperConfig.Encoding.GetString(buffer);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEnumerable<string> ToStrings<T>(IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var type = typeof(T);
            var mapper = FindTypeMapper(type);

            foreach (var obj in source)
            {
                var buffer = mapper.ToByte(mapperConfig.Encoding, false, obj);
                yield return mapperConfig.Encoding.GetString(buffer);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="stream"></param>
        public void ToStrings<T>(IEnumerable<T> source, StreamWriter stream)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var type = typeof(T);
            var mapper = FindTypeMapper(type);

            foreach (var obj in source)
            {
                var buffer = mapper.ToByte(mapperConfig.Encoding, false, obj);
                stream.WriteLine(mapperConfig.Encoding.GetString(buffer));
            }
        }
    }
}
