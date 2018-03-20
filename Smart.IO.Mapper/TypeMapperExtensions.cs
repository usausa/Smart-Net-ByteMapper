namespace Smart.IO.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public static class TypeMapperExtensions
    {
        //--------------------------------------------------------------------------------
        // FromByte
        //--------------------------------------------------------------------------------

        public static T FromByte<T>(ITypeMapper<T> mapper, byte[] buffer)
            where T : new()
        {
            if (buffer.Length < mapper.Size)
            {
                return default;
            }

            var target = new T();
            mapper.FromByte(buffer, target);
            return target;
        }

        public static T FromByte<T>(ITypeMapper<T> mapper, byte[] buffer, int index)
            where T : new()
        {
            if (buffer.Length < mapper.Size)
            {
                return default;
            }

            var target = new T();
            mapper.FromByte(buffer, index, target);
            return target;
        }

        public static IEnumerable<T> FromBytes<T>(ITypeMapper<T> mapper, IEnumerable<byte[]> source)
            where T : new()
        {
            foreach (var buffer in source)
            {
                if (buffer.Length < mapper.Size)
                {
                    continue;
                }

                var target = new T();
                mapper.FromByte(buffer, target);
                yield return target;
            }
        }

        public static IEnumerable<T> FromBytes<T>(ITypeMapper<T> mapper, IEnumerable<byte[]> source, Func<T> factory)
        {
            foreach (var buffer in source)
            {
                if (buffer.Length < mapper.Size)
                {
                    continue;
                }

                var target = factory();
                mapper.FromByte(buffer, target);
                yield return target;
            }
        }

        public static T FromByte<T>(ITypeMapper<T> mapper, Stream stream)
            where T : new()
        {
            var buffer = new byte[mapper.Size];
            if (stream.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                return default;
            }

            var target = new T();
            mapper.FromByte(buffer, target);
            return target;
        }

        public static T FromByte<T>(ITypeMapper<T> mapper, Stream stream, T target)
            where T : new()
        {
            var buffer = new byte[mapper.Size];
            if (stream.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                return default;
            }

            mapper.FromByte(buffer, target);
            return target;
        }

        public static IEnumerable<T> FromBytes<T>(ITypeMapper<T> mapper, Stream stream)
            where T : new()
        {
            var buffer = new byte[mapper.Size];
            while (stream.Read(buffer, 0, buffer.Length) == buffer.Length)
            {
                if (buffer.Length < mapper.Size)
                {
                    continue;
                }

                var target = new T();
                mapper.FromByte(buffer, target);
                yield return target;
            }
        }

        public static IEnumerable<T> FromBytes<T>(ITypeMapper<T> mapper, Stream stream, Func<T> factory)
        {
            var buffer = new byte[mapper.Size];
            while (stream.Read(buffer, 0, buffer.Length) == buffer.Length)
            {
                if (buffer.Length < mapper.Size)
                {
                    continue;
                }

                var target = factory();
                mapper.FromByte(buffer, target);
                yield return target;
            }
        }

        //--------------------------------------------------------------------------------
        // ToByte
        //--------------------------------------------------------------------------------

        public static byte[] ToByte<T>(ITypeMapper<T> mapper, T target)
        {
            var buffer = new byte[mapper.Size];
            mapper.FromByte(buffer, target);
            return buffer;
        }

        // TODO buffer skip (len ?, offset, ) 1?
        // TODO IE<byte[]> 1
        // TODO Stream write 1,n
    }
}
