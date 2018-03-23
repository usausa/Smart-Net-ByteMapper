namespace Smart.IO.Mapper
{
    using System;

    public interface ITypeMapper
    {
        Type TargetType { get; }

        int Size { get; }
    }

    public interface ITypeMapper<in T> : ITypeMapper
    {
        void FromByte(byte[] buffer, int index, T target);

        void ToByte(byte[] buffer, int index, T target);
    }
}
