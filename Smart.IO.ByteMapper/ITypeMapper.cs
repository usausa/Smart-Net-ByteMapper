namespace Smart.IO.ByteMapper
{
    using System;

    public interface ITypeMapper<in T>
    {
        Type TargetType { get; }

        int Size { get; }

        void FromByte(byte[] buffer, int index, T target);

        void ToByte(byte[] buffer, int index, T target);
    }
}
