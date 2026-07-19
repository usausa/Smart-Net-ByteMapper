namespace Smart.IO.ByteMapper.Converters;

using System;
using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

// Date/time text converter. Null maps to an all-filler field: Read returns null when the field is
// all filler bytes, and Write fills the field with the filler byte for a null value.
// 日時テキストコンバーター。null は全フィラーのフィールドに対応付ける: 全フィラーの読み取りは null を
// 返し、null の書き込みはフィールドをフィラーで埋める。
#pragma warning disable IDE0032
public sealed class DateTimeTextConverter<T>
    where T : struct
{
    private readonly int size;
    private readonly byte filler;
    private readonly Encoding encoding;
    private readonly string format;
    private readonly DateTimeStyles style;
    private readonly IFormatProvider provider;
    // Precomputed max buffer sizes to avoid per-call virtual dispatch on encoding.
    private readonly int readCharCount;
    private readonly int writeCharCount;
    private readonly int writeByteCount;

    public int Size => size;

    public DateTimeTextConverter(
        int length,
        string format,
        byte filler = 0x20,
        int codePage = CodePages.Ascii,
        DateTimeStyles style = DateTimeStyles.None,
        Culture culture = Culture.Invariant)
    {
        size = length;
        this.format = format;
        encoding = ByteMapperHelpers.ResolveEncoding(codePage);
        this.style = style;
        provider = culture == Culture.Invariant ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture;
        this.filler = filler;
        readCharCount = encoding.GetMaxCharCount(size);
        writeCharCount = Math.Max(size, 64);
        writeByteCount = encoding.GetMaxByteCount(writeCharCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? Read(ReadOnlySpan<byte> source)
    {
        // trim filler from right
        var count = size;
        while ((count > 0) && (source[count - 1] == filler))
        {
            count--;
        }

        if (count == 0)
        {
            // An all-filler field represents null. Non-nullable properties receive default via the
            // generator-emitted .GetValueOrDefault().
            // 全フィラーのフィールドは null を表す。非 nullable プロパティにはジェネレーターが付与する
            // .GetValueOrDefault() を通じて default が入る。
            return null;
        }

        if (readCharCount <= ByteMapperHelpers.StackallocCharThreshold)
        {
            return ReadCore(source[..count], stackalloc char[readCharCount]);
        }

        return ReadWithPooledBuffer(source[..count]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(Span<byte> destination, T? value)
    {
        // null is written as an all-filler field / null は全フィラーのフィールドとして書き込む
        if (value is null)
        {
            destination[..size].Fill(filler);
            return;
        }

        if ((writeCharCount <= ByteMapperHelpers.StackallocCharThreshold) && (writeByteCount <= ByteMapperHelpers.StackallocByteThreshold))
        {
            WriteCore(destination, value.GetValueOrDefault(), stackalloc char[writeCharCount], stackalloc byte[writeByteCount]);
        }
        else
        {
            WriteWithPooledBuffer(destination, value.GetValueOrDefault());
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private T ReadCore(ReadOnlySpan<byte> source, Span<char> chars)
    {
        var charCount = encoding.GetChars(source, chars);
        return ParseValue(chars[..charCount]);
    }

    // Keep the rare large-buffer path out of the inlined fast path (try/finally blocks inlining).
    [MethodImpl(MethodImplOptions.NoInlining)]
    private T ReadWithPooledBuffer(ReadOnlySpan<byte> source)
    {
        var chars = ArrayPool<char>.Shared.Rent(readCharCount);
        try
        {
            return ReadCore(source, chars.AsSpan(0, readCharCount));
        }
        finally
        {
            ArrayPool<char>.Shared.Return(chars);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteCore(Span<byte> destination, T value, Span<char> chars, Span<byte> encoded)
    {
        if (!TryFormatValue(value, chars, out var charsWritten))
        {
            destination[..size].Fill(filler);
            return;
        }
        var count = encoding.GetBytes(chars[..charsWritten], encoded);
        var written = Math.Min(count, size);
        encoded[..written].CopyTo(destination[..written]);
        if (written < size)
        {
            destination[written..size].Fill(filler);
        }
    }

    // Keep the rare large-buffer path out of the inlined fast path (try/finally blocks inlining).
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void WriteWithPooledBuffer(Span<byte> destination, T value)
    {
        var chars = ArrayPool<char>.Shared.Rent(writeCharCount);
        var encoded = ArrayPool<byte>.Shared.Rent(writeByteCount);
        try
        {
            WriteCore(destination, value, chars.AsSpan(0, writeCharCount), encoded.AsSpan(0, writeByteCount));
        }
        finally
        {
            ArrayPool<char>.Shared.Return(chars);
            ArrayPool<byte>.Shared.Return(encoded);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private T ParseValue(ReadOnlySpan<char> str)
    {
        if (typeof(T) == typeof(DateTime))
        {
            return Unsafe.BitCast<DateTime, T>(DateTime.ParseExact(str, format, provider, style));
        }

        if (typeof(T) == typeof(DateTimeOffset))
        {
            return Unsafe.BitCast<DateTimeOffset, T>(DateTimeOffset.ParseExact(str, format, provider, style));
        }

        if (typeof(T) == typeof(DateOnly))
        {
            return Unsafe.BitCast<DateOnly, T>(DateOnly.ParseExact(str, format, provider));
        }

        if (typeof(T) == typeof(TimeOnly))
        {
            return Unsafe.BitCast<TimeOnly, T>(TimeOnly.ParseExact(str, format, provider));
        }

        throw new NotSupportedException($"Unsupported type: {typeof(T)}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryFormatValue(T value, Span<char> buffer, out int charsWritten)
    {
        if (typeof(T) == typeof(DateTime))
        {
            return Unsafe.BitCast<T, DateTime>(value).TryFormat(buffer, out charsWritten, format, provider);
        }

        if (typeof(T) == typeof(DateTimeOffset))
        {
            return Unsafe.BitCast<T, DateTimeOffset>(value).TryFormat(buffer, out charsWritten, format, provider);
        }

        if (typeof(T) == typeof(DateOnly))
        {
            return Unsafe.BitCast<T, DateOnly>(value).TryFormat(buffer, out charsWritten, format, provider);
        }

        if (typeof(T) == typeof(TimeOnly))
        {
            return Unsafe.BitCast<T, TimeOnly>(value).TryFormat(buffer, out charsWritten, format, provider);
        }

        throw new NotSupportedException($"Unsupported type: {typeof(T)}");
    }
}
#pragma warning restore IDE0032
