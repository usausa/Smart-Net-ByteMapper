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
        void FromByte(byte[] buffer, T target);

        void ToByte(byte[] buffer, T target);
    }
}
