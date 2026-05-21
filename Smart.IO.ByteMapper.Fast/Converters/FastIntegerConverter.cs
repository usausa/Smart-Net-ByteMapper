namespace Smart.IO.ByteMapper.Fast.Converters;

using System.Runtime.CompilerServices;

using Smart.IO.ByteMapper.Fast.Helpers;

/// <summary>ASCII バイト表現の整数コンバーター。</summary>
public sealed class FastIntegerConverter<T>
    where T : struct
{
    private readonly Padding padding;
    private readonly bool zerofill;
    private readonly byte filler;

    /// <summary>フィールドのバイト長を取得します。</summary>
    public int Size { get; }

    /// <summary><see cref="FastIntegerConverter{T}"/> の新しいインスタンスを初期化します。</summary>
    public FastIntegerConverter(int length, Padding padding = Padding.Left, bool zerofill = false, byte filler = 0x20)
    {
        Size = length;
        this.padding = padding;
        this.zerofill = zerofill;
        this.filler = filler;
    }

    /// <summary>バッファーから整数値を読み取ります。</summary>
    public T? Read(ReadOnlySpan<byte> buffer)
    {
        if (typeof(T) == typeof(int))
        {
            if (FastNumberByteHelper.TryParseInt32(buffer, 0, Size, filler, out var result))
            {
                return Unsafe.As<int, T>(ref result);
            }

            return null;
        }

        if (typeof(T) == typeof(short))
        {
            if (FastNumberByteHelper.TryParseInt32(buffer, 0, Size, filler, out var result))
            {
                var s = (short)result;
                return Unsafe.As<short, T>(ref s);
            }

            return null;
        }

        if (typeof(T) == typeof(long))
        {
            if (FastNumberByteHelper.TryParseInt64(buffer, 0, Size, filler, out var result))
            {
                return Unsafe.As<long, T>(ref result);
            }

            return null;
        }

        return null;
    }

    /// <summary>バッファーへ整数値を書き込みます。</summary>
    public void Write(Span<byte> buffer, T? value)
    {
        if (value is null)
        {
            buffer[..Size].Fill(filler);
            return;
        }

        if (typeof(T) == typeof(int))
        {
            var v = value.Value;
            FastNumberByteHelper.FormatInt32(buffer, 0, Size, Unsafe.As<T, int>(ref v), padding, zerofill, filler);
        }
        else if (typeof(T) == typeof(short))
        {
            var v = value.Value;
            FastNumberByteHelper.FormatInt32(buffer, 0, Size, Unsafe.As<T, short>(ref v), padding, zerofill, filler);
        }
        else if (typeof(T) == typeof(long))
        {
            var v = value.Value;
            FastNumberByteHelper.FormatInt64(buffer, 0, Size, Unsafe.As<T, long>(ref v), padding, zerofill, filler);
        }
    }
}
