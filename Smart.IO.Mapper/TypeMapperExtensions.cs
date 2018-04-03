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

        public static void FromByte<T>(this ITypeMapper<T> mapper, byte[] buffer, T target)
        {
            mapper.FromByte(buffer, 0, target);
        }

        public static T FromByte<T>(this ITypeMapper<T> mapper, byte[] buffer)
            where T : new()
        {
            var target = new T();
            mapper.FromByte(buffer, 0, target);
            return target;
        }

        public static T FromByte<T>(this ITypeMapper<T> mapper, byte[] buffer, int index)
            where T : new()
        {
            var target = new T();
            mapper.FromByte(buffer, index, target);
            return target;
        }

        public static IEnumerable<T> FromByteMultiple<T>(this ITypeMapper<T> mapper, byte[] buffer)
            where T : new()
        {
            return mapper.FromByteMultiple(buffer, 0);
        }

        public static IEnumerable<T> FromByteMultiple<T>(this ITypeMapper<T> mapper, byte[] buffer, int start)
            where T : new()
        {
            while (start + mapper.Size <= buffer.Length)
            {
                var target = new T();
                mapper.FromByte(buffer, start, target);
                yield return target;

                start += buffer.Length;
            }
        }

        public static IEnumerable<T> FromByteMultiple<T>(this ITypeMapper<T> mapper, byte[] buffer, Func<T> factory)
        {
            return mapper.FromByteMultiple(buffer, 0, factory);
        }

        public static IEnumerable<T> FromByteMultiple<T>(this ITypeMapper<T> mapper, byte[] buffer, int start, Func<T> factory)
        {
            while (start + mapper.Size <= buffer.Length)
            {
                var target = factory();
                mapper.FromByte(buffer, start, target);
                yield return target;

                start += buffer.Length;
            }
        }

        public static IEnumerable<T> FromByteMultiple<T>(this ITypeMapper<T> mapper, IEnumerable<byte[]> source)
            where T : new()
        {
            foreach (var buffer in source)
            {
                var target = new T();
                mapper.FromByte(buffer, 0, target);
                yield return target;
            }
        }

        public static IEnumerable<T> FromByteMultiple<T>(this ITypeMapper<T> mapper, IEnumerable<byte[]> source, Func<T> factory)
        {
            foreach (var buffer in source)
            {
                var target = factory();
                mapper.FromByte(buffer, 0, target);
                yield return target;
            }
        }

        public static T FromStream<T>(this ITypeMapper<T> mapper, Stream stream)
            where T : new()
        {
            var buffer = new byte[mapper.Size];
            if (stream.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                return default;
            }

            var target = new T();
            mapper.FromByte(buffer, 0, target);
            return target;
        }

        public static bool FromStream<T>(this ITypeMapper<T> mapper, Stream stream, T target)
        {
            var buffer = new byte[mapper.Size];
            if (stream.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                return false;
            }

            mapper.FromByte(buffer, 0, target);
            return true;
        }

        public static IEnumerable<T> FromStreamMultiple<T>(this ITypeMapper<T> mapper, Stream stream)
            where T : new()
        {
            var buffer = new byte[mapper.Size];
            while (stream.Read(buffer, 0, buffer.Length) == buffer.Length)
            {
                var target = new T();
                mapper.FromByte(buffer, 0, target);
                yield return target;
            }
        }

        public static IEnumerable<T> FromStreamMultiple<T>(this ITypeMapper<T> mapper, Stream stream, Func<T> factory)
        {
            var buffer = new byte[mapper.Size];
            while (stream.Read(buffer, 0, buffer.Length) == buffer.Length)
            {
                var target = factory();
                mapper.FromByte(buffer, 0, target);
                yield return target;
            }
        }

        //--------------------------------------------------------------------------------
        // ToByte
        //--------------------------------------------------------------------------------

        public static void ToByte<T>(this ITypeMapper<T> mapper, byte[] buffer, T target)
        {
            mapper.ToByte(buffer, 0, target);
        }

        public static byte[] ToByte<T>(this ITypeMapper<T> mapper, T target)
        {
            var buffer = new byte[mapper.Size];
            mapper.ToByte(buffer, 0, target);
            return buffer;
        }

        public static byte[] ToByteMultiple<T>(this ITypeMapper<T> mapper, IEnumerable<T> source)
        {
            if (source is ICollection<T> collection)
            {
                var buffer = new byte[mapper.Size * collection.Count];
                mapper.ToByteMultiple(buffer, 0, source);
                return buffer;
            }

            using (var ms = new MemoryStream())
            {
                mapper.ToStreamMultiple(ms, source);
                return ms.ToArray();
            }
        }

        public static void ToByteMultiple<T>(this ITypeMapper<T> mapper, byte[] buffer, IEnumerable<T> source)
        {
            mapper.ToByteMultiple(buffer, 0, source);
        }

        public static void ToByteMultiple<T>(this ITypeMapper<T> mapper, byte[] buffer, int start, IEnumerable<T> source)
        {
            foreach (var target in source)
            {
                mapper.ToByte(buffer, start, target);
                start += mapper.Size;
            }
        }

        public static void ToStream<T>(this ITypeMapper<T> mapper, Stream stream, T target)
        {
            var buffer = new byte[mapper.Size];
            mapper.ToByte(buffer, 0, target);
            stream.Write(buffer, 0, buffer.Length);
        }

        public static void ToStreamMultiple<T>(this ITypeMapper<T> mapper, Stream stream, IEnumerable<T> source)
        {
            var buffer = new byte[mapper.Size];
            foreach (var target in source)
            {
                mapper.ToByte(buffer, 0, target);
                stream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
