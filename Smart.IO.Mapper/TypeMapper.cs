namespace Smart.IO.Mapper
{
    using System;
    using System.Linq;

    public class TypeMapper<T> : ITypeMapper<T>
    {
        private readonly IMemberMapper[] readableMappers;

        private readonly IMemberMapper[] writableMappers;

        public Type TargetType { get; }

        public int Size { get; }

        public TypeMapper(Type targetType, int size, IMemberMapper[] mappers)
        {
            TargetType = targetType;
            Size = size;
            readableMappers = mappers.Where(x => x.CanRead).ToArray();
            writableMappers = mappers.Where(x => x.CanWrite).ToArray();
        }

        public void FromByte(byte[] buffer, int index, T target)
        {
            for (var i = 0; i < readableMappers.Length; i++)
            {
                readableMappers[i].Read(buffer, index, target);
            }
        }

        public void ToByte(byte[] buffer, int index, T target)
        {
            for (var i = 0; i < writableMappers.Length; i++)
            {
                writableMappers[i].Write(buffer, index, target);
            }
        }
    }
}
