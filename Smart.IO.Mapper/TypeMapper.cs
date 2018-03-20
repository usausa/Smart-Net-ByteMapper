namespace Smart.IO.Mapper
{
    using System;

    public class TypeMapper<T> : ITypeMapper<T>
    {
        private readonly IMemberMapper[] mappers;

        public Type TargetType { get; }

        public int Size { get; }

        public TypeMapper(Type targetType, int size, IMemberMapper[] mappers)
        {
            TargetType = targetType;
            Size = size;
            this.mappers = mappers;
        }

        public void FromByte(byte[] buffer, T target)
        {
            for (var i = 0; i < mappers.Length; i++)
            {
                mappers[i].Read(buffer, 0, target);
            }
        }

        public void FromByte(byte[] buffer, int index, T target)
        {
            for (var i = 0; i < mappers.Length; i++)
            {
                mappers[i].Read(buffer, index, target);
            }
        }

        public void ToByte(byte[] buffer, T target)
        {
            for (var i = 0; i < mappers.Length; i++)
            {
                mappers[i].Write(buffer, 0, target);
            }
        }

        public void ToByte(byte[] buffer, int index, T target)
        {
            for (var i = 0; i < mappers.Length; i++)
            {
                mappers[i].Write(buffer, index, target);
            }
        }
    }
}
