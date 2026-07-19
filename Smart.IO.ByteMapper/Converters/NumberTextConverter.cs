namespace Smart.IO.ByteMapper.Converters;

using System;
using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

// Number text converter. Null maps to an all-filler field: Read returns null when the trimmed field
// is empty (all filler bytes), and Write fills the field with the filler byte for a null value.
// Null detection requires trim (the default); with trim disabled an all-filler field fails to parse.
// 数値テキストコンバーター。null は全フィラーのフィールドに対応付ける: トリム後が空（全フィラー）の
// 読み取りは null を返し、null の書き込みはフィールドをフィラーで埋める。
// null 判定は trim（既定で有効）が前提で、trim 無効時の全フィラーはパース失敗になる。
#pragma warning disable IDE0032
public sealed class NumberTextConverter<T>
    where T : struct
{
    private readonly int size;
    private readonly bool trim;
    private readonly Padding padding;
    private readonly byte filler;
    private readonly Encoding encoding;
    private readonly string? format;
    private readonly NumberStyles style;
    private readonly IFormatProvider provider;
    // Precomputed max buffer sizes to avoid per-call virtual dispatch on encoding.
    private readonly int readCharCount;
    private readonly int writeCharCount;
    private readonly int writeByteCount;

    public int Size => size;

    public NumberTextConverter(
        int length,
        bool trim = true,
        Padding padding = Padding.Left,
        byte filler = 0x20,
        int codePage = CodePages.Ascii,
        string? format = null,
        NumberStyles style = NumberStyles.Integer,
        Culture culture = Culture.Invariant)
    {
        size = length;
        this.format = format;
        encoding = ByteMapperHelpers.ResolveEncoding(codePage);
        this.trim = trim;
        this.padding = padding;
        this.filler = filler;
        this.style = style;
        provider = culture == Culture.Invariant ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture;
        readCharCount = encoding.GetMaxCharCount(size);
        writeCharCount = Math.Max(size, 48);
        writeByteCount = encoding.GetMaxByteCount(writeCharCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? Read(ReadOnlySpan<byte> source)
    {
        var start = 0;
        var count = size;
        if (trim)
        {
            ByteMapperHelpers.TrimRange(source, ref start, ref count, padding, filler);
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
            return ReadCore(source.Slice(start, count), stackalloc char[readCharCount]);
        }

        return ReadWithPooledBuffer(source.Slice(start, count));
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
        ByteMapperHelpers.CopyWithPadding(encoded[..count], destination, Size, padding, filler);
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
        if (typeof(T) == typeof(int))
        {
            return Unsafe.BitCast<int, T>(Int32.Parse(str, style, provider));
        }

        if (typeof(T) == typeof(long))
        {
            return Unsafe.BitCast<long, T>(Int64.Parse(str, style, provider));
        }

        if (typeof(T) == typeof(short))
        {
            return Unsafe.BitCast<short, T>(Int16.Parse(str, style, provider));
        }

        if (typeof(T) == typeof(float))
        {
            return Unsafe.BitCast<float, T>(Single.Parse(str, style, provider));
        }

        if (typeof(T) == typeof(double))
        {
            return Unsafe.BitCast<double, T>(Double.Parse(str, style, provider));
        }

        if (typeof(T) == typeof(decimal))
        {
            return Unsafe.BitCast<decimal, T>(Decimal.Parse(str, style, provider));
        }

        throw new NotSupportedException($"Unsupported type: {typeof(T)}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryFormatValue(T value, Span<char> buffer, out int charsWritten)
    {
        if (typeof(T) == typeof(int))
        {
            return Unsafe.BitCast<T, int>(value).TryFormat(buffer, out charsWritten, format, provider);
        }

        if (typeof(T) == typeof(long))
        {
            return Unsafe.BitCast<T, long>(value).TryFormat(buffer, out charsWritten, format, provider);
        }

        if (typeof(T) == typeof(short))
        {
            return Unsafe.BitCast<T, short>(value).TryFormat(buffer, out charsWritten, format, provider);
        }

        if (typeof(T) == typeof(float))
        {
            return Unsafe.BitCast<T, float>(value).TryFormat(buffer, out charsWritten, format, provider);
        }

        if (typeof(T) == typeof(double))
        {
            return Unsafe.BitCast<T, double>(value).TryFormat(buffer, out charsWritten, format, provider);
        }

        if (typeof(T) == typeof(decimal))
        {
            return Unsafe.BitCast<T, decimal>(value).TryFormat(buffer, out charsWritten, format, provider);
        }

        throw new NotSupportedException($"Unsupported type: {typeof(T)}");
    }
}
#pragma warning restore IDE0032
