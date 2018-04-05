namespace Smart.IO.Mapper
{
    using System;

    public interface ITypeMapper<T>
    {
        Type TargetType { get; }

        int Size { get; }

        void FromByte(byte[] buffer, int index, T target);

        void ToByte(byte[] buffer, int index, T target);
    }
}
