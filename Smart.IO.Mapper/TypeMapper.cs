namespace Smart.IO.Mapper
{
    using System;
    using System.Linq;

    using Smart.IO.Mapper.Mappings;

    public class TypeMapper<T> : ITypeMapper<T>
    {
        private readonly IMapping[] readableMappings;

        private readonly IMapping[] writableMappings;

        public Type TargetType { get; }

        public int Size { get; }

        public TypeMapper(Type targetType, int size, IMapping[] mappings)
        {
            TargetType = targetType;
            Size = size;
            readableMappings = mappings.Where(x => x.CanRead).ToArray();
            writableMappings = mappings.Where(x => x.CanWrite).ToArray();
        }

        public void FromByte(byte[] buffer, int index, T target)
        {
            for (var i = 0; i < readableMappings.Length; i++)
            {
                readableMappings[i].Read(buffer, index, target);
            }
        }

        public void ToByte(byte[] buffer, int index, T target)
        {
            for (var i = 0; i < writableMappings.Length; i++)
            {
                writableMappings[i].Write(buffer, index, target);
            }
        }
    }
}
