namespace Smart.IO.Mapper
{
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

        // TODO Stream read ?
        // TODO Writer, bytes & stream ?
        // TODO T, byte[]を返す拡張メソッド
    }
}
