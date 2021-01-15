namespace Smart.IO.ByteMapper
{
    using System;

    public interface ITypeMapper
    {
        Type TargetType { get; }

        int Size { get; }

        void FromByte(byte[] buffer, int index, object target);

        void ToByte(byte[] buffer, int index, object target);
    }

    public interface ITypeMapper<in T> : ITypeMapper
    {
        void FromByte(byte[] buffer, int index, T target);

        void ToByte(byte[] buffer, int index, T target);
    }
}
