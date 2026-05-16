namespace Smart.IO.ByteMapper.Converters;

using System.Runtime.CompilerServices;

using Smart.IO.ByteMapper.Helpers;

//--------------------------------------------------------------------------------
// Guid
//--------------------------------------------------------------------------------

/// <summary>
/// Reads/writes a <see cref="Guid"/> as 16 bytes in big-endian (network) byte order.
/// </summary>
internal sealed class BigEndianGuidBinaryConverter : IMapConverter
{
    public static BigEndianGuidBinaryConverter Default { get; } = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object Read(ReadOnlySpan<byte> buffer)
    {
        // Reinterpret the 16 raw bytes as a Guid stored in big-endian layout.
        // Guid's internal layout is LE, so we reverse the three multi-byte sub-fields.
        Span<byte> tmp = stackalloc byte[16];
        buffer[..16].CopyTo(tmp);
        ReverseBigEndianToGuid(tmp);
        return new Guid(tmp);
    }

    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..16], 0);
            return;
        }

        ((Guid)value).TryWriteBytes(buffer[..16]);
        ReverseGuidToBigEndian(buffer[..16]);
    }

    // Guid internal layout: Data1(4 LE), Data2(2 LE), Data3(2 LE), Data4(8 raw)
    // Convert to/from big-endian (network) order by reversing those sub-fields.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ReverseBigEndianToGuid(Span<byte> b)
    {
        (b[0], b[3]) = (b[3], b[0]);
        (b[1], b[2]) = (b[2], b[1]);
        (b[4], b[5]) = (b[5], b[4]);
        (b[6], b[7]) = (b[7], b[6]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ReverseGuidToBigEndian(Span<byte> b)
    {
        (b[0], b[3]) = (b[3], b[0]);
        (b[1], b[2]) = (b[2], b[1]);
        (b[4], b[5]) = (b[5], b[4]);
        (b[6], b[7]) = (b[7], b[6]);
    }
}

/// <summary>
/// Reads/writes a <see cref="Guid"/> as 16 bytes in little-endian (native .NET) byte order.
/// </summary>
internal sealed class LittleEndianGuidBinaryConverter : IMapConverter
{
    public static LittleEndianGuidBinaryConverter Default { get; } = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object Read(ReadOnlySpan<byte> buffer) =>
        new Guid(buffer[..16]);

    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..16], 0);
            return;
        }

        ((Guid)value).TryWriteBytes(buffer[..16]);
    }
}

//--------------------------------------------------------------------------------
// Nullable Guid
//--------------------------------------------------------------------------------

internal sealed class BigEndianNullableGuidBinaryConverter : IMapConverter
{
    private static readonly byte[] NullBytes = new byte[16];

    public static BigEndianNullableGuidBinaryConverter Default { get; } = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object Read(ReadOnlySpan<byte> buffer)
    {
        if (buffer[..16].SequenceEqual(NullBytes))
        {
            return null;
        }

        return (Guid?)((Guid)BigEndianGuidBinaryConverter.Default.Read(buffer));
    }

    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..16], 0);
            return;
        }

        BigEndianGuidBinaryConverter.Default.Write(buffer, (Guid)(Guid?)value);
    }
}

internal sealed class LittleEndianNullableGuidBinaryConverter : IMapConverter
{
    private static readonly byte[] NullBytes = new byte[16];

    public static LittleEndianNullableGuidBinaryConverter Default { get; } = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object Read(ReadOnlySpan<byte> buffer)
    {
        if (buffer[..16].SequenceEqual(NullBytes))
        {
            return null;
        }

        return (Guid?)new Guid(buffer[..16]);
    }

    public void Write(Span<byte> buffer, object value)
    {
        if (value is null)
        {
            BytesHelper.Fill(buffer[..16], 0);
            return;
        }

        ((Guid)(Guid?)value).TryWriteBytes(buffer[..16]);
    }
}
