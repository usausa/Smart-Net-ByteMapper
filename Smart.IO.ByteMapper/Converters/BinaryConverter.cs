namespace Smart.IO.ByteMapper.Converters;

using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

/// <summary>
/// Binary converter for unmanaged types using BinaryPrimitives.
/// </summary>
public sealed class BinaryConverter<T>
    where T : unmanaged
{
    private readonly Endian endian;

    public static readonly int Size = Unsafe.SizeOf<T>();

    public BinaryConverter(Endian endian = Endian.Big)
    {
        this.endian = endian;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Read(ReadOnlySpan<byte> source)
    {
        if (typeof(T) == typeof(byte)) return (T)(object)source[0];
        if (typeof(T) == typeof(sbyte)) return (T)(object)(sbyte)source[0];
        if (typeof(T) == typeof(short)) return endian == Endian.Big
            ? (T)(object)BinaryPrimitives.ReadInt16BigEndian(source)
            : (T)(object)BinaryPrimitives.ReadInt16LittleEndian(source);
        if (typeof(T) == typeof(ushort)) return endian == Endian.Big
            ? (T)(object)BinaryPrimitives.ReadUInt16BigEndian(source)
            : (T)(object)BinaryPrimitives.ReadUInt16LittleEndian(source);
        if (typeof(T) == typeof(int)) return endian == Endian.Big
            ? (T)(object)BinaryPrimitives.ReadInt32BigEndian(source)
            : (T)(object)BinaryPrimitives.ReadInt32LittleEndian(source);
        if (typeof(T) == typeof(uint)) return endian == Endian.Big
            ? (T)(object)BinaryPrimitives.ReadUInt32BigEndian(source)
            : (T)(object)BinaryPrimitives.ReadUInt32LittleEndian(source);
        if (typeof(T) == typeof(long)) return endian == Endian.Big
            ? (T)(object)BinaryPrimitives.ReadInt64BigEndian(source)
            : (T)(object)BinaryPrimitives.ReadInt64LittleEndian(source);
        if (typeof(T) == typeof(ulong)) return endian == Endian.Big
            ? (T)(object)BinaryPrimitives.ReadUInt64BigEndian(source)
            : (T)(object)BinaryPrimitives.ReadUInt64LittleEndian(source);
        if (typeof(T) == typeof(float))
        {
            var bits = endian == Endian.Big
                ? BinaryPrimitives.ReadInt32BigEndian(source)
                : BinaryPrimitives.ReadInt32LittleEndian(source);
            return (T)(object)BitConverter.Int32BitsToSingle(bits);
        }
        if (typeof(T) == typeof(double))
        {
            var bits = endian == Endian.Big
                ? BinaryPrimitives.ReadInt64BigEndian(source)
                : BinaryPrimitives.ReadInt64LittleEndian(source);
            return (T)(object)BitConverter.Int64BitsToDouble(bits);
        }
        if (typeof(T) == typeof(decimal))
        {
            Span<int> parts = stackalloc int[4];
            for (var i = 0; i < 4; i++)
            {
                parts[i] = endian == Endian.Big
                    ? BinaryPrimitives.ReadInt32BigEndian(source.Slice(i * 4, 4))
                    : BinaryPrimitives.ReadInt32LittleEndian(source.Slice(i * 4, 4));
            }
            return (T)(object)new decimal(parts[0], parts[1], parts[2], (parts[3] & unchecked((int)0x80000000)) != 0, (byte)((parts[3] >> 16) & 0x7F));
        }
        throw new NotSupportedException($"Unsupported type: {typeof(T)}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(Span<byte> destination, T value)
    {
        if (typeof(T) == typeof(byte)) { destination[0] = (byte)(object)value; return; }
        if (typeof(T) == typeof(sbyte)) { destination[0] = (byte)(sbyte)(object)value; return; }
        if (typeof(T) == typeof(short))
        {
            if (endian == Endian.Big) BinaryPrimitives.WriteInt16BigEndian(destination, (short)(object)value);
            else BinaryPrimitives.WriteInt16LittleEndian(destination, (short)(object)value);
            return;
        }
        if (typeof(T) == typeof(ushort))
        {
            if (endian == Endian.Big) BinaryPrimitives.WriteUInt16BigEndian(destination, (ushort)(object)value);
            else BinaryPrimitives.WriteUInt16LittleEndian(destination, (ushort)(object)value);
            return;
        }
        if (typeof(T) == typeof(int))
        {
            if (endian == Endian.Big) BinaryPrimitives.WriteInt32BigEndian(destination, (int)(object)value);
            else BinaryPrimitives.WriteInt32LittleEndian(destination, (int)(object)value);
            return;
        }
        if (typeof(T) == typeof(uint))
        {
            if (endian == Endian.Big) BinaryPrimitives.WriteUInt32BigEndian(destination, (uint)(object)value);
            else BinaryPrimitives.WriteUInt32LittleEndian(destination, (uint)(object)value);
            return;
        }
        if (typeof(T) == typeof(long))
        {
            if (endian == Endian.Big) BinaryPrimitives.WriteInt64BigEndian(destination, (long)(object)value);
            else BinaryPrimitives.WriteInt64LittleEndian(destination, (long)(object)value);
            return;
        }
        if (typeof(T) == typeof(ulong))
        {
            if (endian == Endian.Big) BinaryPrimitives.WriteUInt64BigEndian(destination, (ulong)(object)value);
            else BinaryPrimitives.WriteUInt64LittleEndian(destination, (ulong)(object)value);
            return;
        }
        if (typeof(T) == typeof(float))
        {
            var bits = BitConverter.SingleToInt32Bits((float)(object)value);
            if (endian == Endian.Big) BinaryPrimitives.WriteInt32BigEndian(destination, bits);
            else BinaryPrimitives.WriteInt32LittleEndian(destination, bits);
            return;
        }
        if (typeof(T) == typeof(double))
        {
            var bits = BitConverter.DoubleToInt64Bits((double)(object)value);
            if (endian == Endian.Big) BinaryPrimitives.WriteInt64BigEndian(destination, bits);
            else BinaryPrimitives.WriteInt64LittleEndian(destination, bits);
            return;
        }
        if (typeof(T) == typeof(decimal))
        {
            var d = (decimal)(object)value;
            Span<int> parts = stackalloc int[4];
            decimal.GetBits(d, parts);
            for (var i = 0; i < 4; i++)
            {
                if (endian == Endian.Big) BinaryPrimitives.WriteInt32BigEndian(destination.Slice(i * 4, 4), parts[i]);
                else BinaryPrimitives.WriteInt32LittleEndian(destination.Slice(i * 4, 4), parts[i]);
            }
            return;
        }
        throw new NotSupportedException($"Unsupported type: {typeof(T)}");
    }
}
