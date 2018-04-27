namespace Smart.IO.ByteMapper
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public static class TypeMapperExtensions
    {
        //--------------------------------------------------------------------------------
        // FromByte
        //--------------------------------------------------------------------------------

        public static void FromByte<T>(this ITypeMapper<T> mapper, byte[] buffer, T target)
        {
            mapper.FromByte(buffer, 0, target);
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

        public static IEnumerable<T> FromByteMultiple<T>(this ITypeMapper<T> mapper, IEnumerable<byte[]> source, Func<T> factory)
        {
            foreach (var buffer in source)
            {
                var target = factory();
                mapper.FromByte(buffer, 0, target);
                yield return target;
            }
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
        // FromByte NonGeneric
        //--------------------------------------------------------------------------------

        public static void FromByte(this ITypeMapper mapper, byte[] buffer, object target)
        {
            mapper.FromByte(buffer, 0, target);
        }

        public static IEnumerable<object> FromByteMultiple(this ITypeMapper mapper, byte[] buffer, Func<object> factory)
        {
            return mapper.FromByteMultiple(buffer, 0, factory);
        }

        public static IEnumerable<object> FromByteMultiple(this ITypeMapper mapper, byte[] buffer, int start, Func<object> factory)
        {
            while (start + mapper.Size <= buffer.Length)
            {
                var target = factory();
                mapper.FromByte(buffer, start, target);
                yield return target;

                start += buffer.Length;
            }
        }

        public static IEnumerable<object> FromByteMultiple(this ITypeMapper mapper, IEnumerable<byte[]> source, Func<object> factory)
        {
            foreach (var buffer in source)
            {
                var target = factory();
                mapper.FromByte(buffer, 0, target);
                yield return target;
            }
        }

        public static bool FromStream(this ITypeMapper mapper, Stream stream, object target)
        {
            var buffer = new byte[mapper.Size];
            if (stream.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                return false;
            }

            mapper.FromByte(buffer, 0, target);
            return true;
        }

        public static IEnumerable<object> FromStreamMultiple(this ITypeMapper mapper, Stream stream, Func<object> factory)
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
            if (source is ICollection collection)
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

        public static Task ToStreamAsync<T>(this ITypeMapper<T> mapper, Stream stream, T target)
        {
            var buffer = new byte[mapper.Size];
            mapper.ToByte(buffer, 0, target);
            return stream.WriteAsync(buffer, 0, buffer.Length);
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

        public static async Task ToStreamMultipleAsync<T>(this ITypeMapper<T> mapper, Stream stream, IEnumerable<T> source)
        {
            var buffer = new byte[mapper.Size];
            foreach (var target in source)
            {
                mapper.ToByte(buffer, 0, target);
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }
        }

        //--------------------------------------------------------------------------------
        // ToByte NonGeneric
        //--------------------------------------------------------------------------------

        public static void ToByte(this ITypeMapper mapper, byte[] buffer, object target)
        {
            mapper.ToByte(buffer, 0, target);
        }

        public static byte[] ToByte(this ITypeMapper mapper, object target)
        {
            var buffer = new byte[mapper.Size];
            mapper.ToByte(buffer, 0, target);
            return buffer;
        }

        public static byte[] ToByteMultiple(this ITypeMapper mapper, IEnumerable<object> source)
        {
            if (source is ICollection collection)
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

        public static void ToByteMultiple(this ITypeMapper mapper, byte[] buffer, IEnumerable source)
        {
            mapper.ToByteMultiple(buffer, 0, source);
        }

        public static void ToByteMultiple(this ITypeMapper mapper, byte[] buffer, int start, IEnumerable source)
        {
            foreach (var target in source)
            {
                mapper.ToByte(buffer, start, target);
                start += mapper.Size;
            }
        }

        public static void ToStream(this ITypeMapper mapper, Stream stream, object target)
        {
            var buffer = new byte[mapper.Size];
            mapper.ToByte(buffer, 0, target);
            stream.Write(buffer, 0, buffer.Length);
        }

        public static Task ToStreamAsync(this ITypeMapper mapper, Stream stream, object target)
        {
            var buffer = new byte[mapper.Size];
            mapper.ToByte(buffer, 0, target);
            return stream.WriteAsync(buffer, 0, buffer.Length);
        }

        public static void ToStreamMultiple(this ITypeMapper mapper, Stream stream, IEnumerable source)
        {
            var buffer = new byte[mapper.Size];
            foreach (var target in source)
            {
                mapper.ToByte(buffer, 0, target);
                stream.Write(buffer, 0, buffer.Length);
            }
        }

        public static async Task ToStreamMultipleAsync(this ITypeMapper mapper, Stream stream, IEnumerable source)
        {
            var buffer = new byte[mapper.Size];
            foreach (var target in source)
            {
                mapper.ToByte(buffer, 0, target);
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }
        }
    }
}
