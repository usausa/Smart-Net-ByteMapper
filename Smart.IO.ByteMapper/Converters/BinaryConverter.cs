namespace Smart.IO.ByteMapper.Converters;

using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

// Binary converter for unmanaged types using BinaryPrimitives.
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
        if (typeof(T) == typeof(byte))
        {
            return Unsafe.BitCast<byte, T>(source[0]);
        }

        if (typeof(T) == typeof(sbyte))
        {
            return Unsafe.BitCast<sbyte, T>((sbyte)source[0]);
        }

        if (typeof(T) == typeof(short))
        {
            var v = endian == Endian.Big
                ? BinaryPrimitives.ReadInt16BigEndian(source)
                : BinaryPrimitives.ReadInt16LittleEndian(source);
            return Unsafe.BitCast<short, T>(v);
        }

        if (typeof(T) == typeof(ushort))
        {
            var v = endian == Endian.Big
                ? BinaryPrimitives.ReadUInt16BigEndian(source)
                : BinaryPrimitives.ReadUInt16LittleEndian(source);
            return Unsafe.BitCast<ushort, T>(v);
        }

        if (typeof(T) == typeof(int))
        {
            var v = endian == Endian.Big
                ? BinaryPrimitives.ReadInt32BigEndian(source)
                : BinaryPrimitives.ReadInt32LittleEndian(source);
            return Unsafe.BitCast<int, T>(v);
        }

        if (typeof(T) == typeof(uint))
        {
            var v = endian == Endian.Big
                ? BinaryPrimitives.ReadUInt32BigEndian(source)
                : BinaryPrimitives.ReadUInt32LittleEndian(source);
            return Unsafe.BitCast<uint, T>(v);
        }

        if (typeof(T) == typeof(long))
        {
            var v = endian == Endian.Big
                ? BinaryPrimitives.ReadInt64BigEndian(source)
                : BinaryPrimitives.ReadInt64LittleEndian(source);
            return Unsafe.BitCast<long, T>(v);
        }

        if (typeof(T) == typeof(ulong))
        {
            var v = endian == Endian.Big
                ? BinaryPrimitives.ReadUInt64BigEndian(source)
                : BinaryPrimitives.ReadUInt64LittleEndian(source);
            return Unsafe.BitCast<ulong, T>(v);
        }

        if (typeof(T) == typeof(float))
        {
            var bits = endian == Endian.Big
                ? BinaryPrimitives.ReadInt32BigEndian(source)
                : BinaryPrimitives.ReadInt32LittleEndian(source);
            return Unsafe.BitCast<float, T>(BitConverter.Int32BitsToSingle(bits));
        }

        if (typeof(T) == typeof(double))
        {
            var bits = endian == Endian.Big
                ? BinaryPrimitives.ReadInt64BigEndian(source)
                : BinaryPrimitives.ReadInt64LittleEndian(source);
            return Unsafe.BitCast<double, T>(BitConverter.Int64BitsToDouble(bits));
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
            var d = new decimal(parts[0], parts[1], parts[2], (parts[3] & unchecked((int)0x80000000)) != 0, (byte)((parts[3] >> 16) & 0x7F));
            return Unsafe.BitCast<decimal, T>(d);
        }

        throw new NotSupportedException($"Unsupported type: {typeof(T)}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(Span<byte> destination, T value)
    {
        if (typeof(T) == typeof(byte))
        {
            destination[0] = Unsafe.BitCast<T, byte>(value);
            return;
        }

        if (typeof(T) == typeof(sbyte))
        {
            destination[0] = (byte)Unsafe.BitCast<T, sbyte>(value);
            return;
        }

        if (typeof(T) == typeof(short))
        {
            if (endian == Endian.Big)
            {
                BinaryPrimitives.WriteInt16BigEndian(destination, Unsafe.BitCast<T, short>(value));
            }
            else
            {
                BinaryPrimitives.WriteInt16LittleEndian(destination, Unsafe.BitCast<T, short>(value));
            }
            return;
        }

        if (typeof(T) == typeof(ushort))
        {
            if (endian == Endian.Big)
            {
                BinaryPrimitives.WriteUInt16BigEndian(destination, Unsafe.BitCast<T, ushort>(value));
            }
            else
            {
                BinaryPrimitives.WriteUInt16LittleEndian(destination, Unsafe.BitCast<T, ushort>(value));
            }
            return;
        }

        if (typeof(T) == typeof(int))
        {
            if (endian == Endian.Big)
            {
                BinaryPrimitives.WriteInt32BigEndian(destination, Unsafe.BitCast<T, int>(value));
            }
            else
            {
                BinaryPrimitives.WriteInt32LittleEndian(destination, Unsafe.BitCast<T, int>(value));
            }
            return;
        }

        if (typeof(T) == typeof(uint))
        {
            if (endian == Endian.Big)
            {
                BinaryPrimitives.WriteUInt32BigEndian(destination, Unsafe.BitCast<T, uint>(value));
            }
            else
            {
                BinaryPrimitives.WriteUInt32LittleEndian(destination, Unsafe.BitCast<T, uint>(value));
            }
            return;
        }

        if (typeof(T) == typeof(long))
        {
            if (endian == Endian.Big)
            {
                BinaryPrimitives.WriteInt64BigEndian(destination, Unsafe.BitCast<T, long>(value));
            }
            else
            {
                BinaryPrimitives.WriteInt64LittleEndian(destination, Unsafe.BitCast<T, long>(value));
            }
            return;
        }

        if (typeof(T) == typeof(ulong))
        {
            if (endian == Endian.Big)
            {
                BinaryPrimitives.WriteUInt64BigEndian(destination, Unsafe.BitCast<T, ulong>(value));
            }
            else
            {
                BinaryPrimitives.WriteUInt64LittleEndian(destination, Unsafe.BitCast<T, ulong>(value));
            }
            return;
        }

        if (typeof(T) == typeof(float))
        {
            var bits = BitConverter.SingleToInt32Bits(Unsafe.BitCast<T, float>(value));
            if (endian == Endian.Big)
            {
                BinaryPrimitives.WriteInt32BigEndian(destination, bits);
            }
            else
            {
                BinaryPrimitives.WriteInt32LittleEndian(destination, bits);
            }
            return;
        }

        if (typeof(T) == typeof(double))
        {
            var bits = BitConverter.DoubleToInt64Bits(Unsafe.BitCast<T, double>(value));
            if (endian == Endian.Big)
            {
                BinaryPrimitives.WriteInt64BigEndian(destination, bits);
            }
            else
            {
                BinaryPrimitives.WriteInt64LittleEndian(destination, bits);
            }
            return;
        }

        if (typeof(T) == typeof(decimal))
        {
            var d = Unsafe.BitCast<T, decimal>(value);
            Span<int> parts = stackalloc int[4];
            decimal.GetBits(d, parts);
            for (var i = 0; i < 4; i++)
            {
                if (endian == Endian.Big)
                {
                    BinaryPrimitives.WriteInt32BigEndian(destination.Slice(i * 4, 4), parts[i]);
                }
                else
                {
                    BinaryPrimitives.WriteInt32LittleEndian(destination.Slice(i * 4, 4), parts[i]);
                }
            }
            return;
        }

        throw new NotSupportedException($"Unsupported type: {typeof(T)}");
    }
}
