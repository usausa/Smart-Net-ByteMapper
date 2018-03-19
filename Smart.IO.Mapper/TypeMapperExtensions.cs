namespace Smart.IO.Mapper
{
    using System.Collections.Generic;
    using System.IO;

    public static class TypeMapperExtensions
    {
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

        // TODO buffer skip
        // TODO IE<byte[]>
        // TODO Stream write
    }
}
