namespace Smart.IO.ByteMapper.Options.Converters;

using Smart.IO.ByteMapper.Options.Helpers;

/// <summary>ASCII バイト表現の整数コンバーター。</summary>
public sealed class IntegerConverter<T>
    where T : struct
{
    private readonly Padding padding;
    private readonly bool zerofill;
    private readonly byte filler;

    /// <summary>フィールドのバイト長を取得します。</summary>
    public int Size { get; }

    /// <summary><see cref="IntegerConverter{T}"/> の新しいインスタンスを初期化します。</summary>
    public IntegerConverter(int length, Padding padding = Padding.Left, bool zerofill = false, byte filler = 0x20)
    {
        Size = length;
        this.padding = padding;
        this.zerofill = zerofill;
        this.filler = filler;
    }

    /// <summary>バッファーから整数値を読み取ります。</summary>
    public T? Read(ReadOnlySpan<byte> buffer)
    {
        if (typeof(T) == typeof(int) || typeof(T) == typeof(short))
        {
            if (NumberByteHelper.TryParseInt32(buffer, 0, Size, filler, out var result))
            {
                return (T)(object)result;
            }

            return null;
        }

        if (typeof(T) == typeof(long))
        {
            if (NumberByteHelper.TryParseInt64(buffer, 0, Size, filler, out var result))
            {
                return (T)(object)result;
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
            NumberByteHelper.FormatInt32(buffer, 0, Size, (int)(object)value, padding, zerofill, filler);
        }
        else if (typeof(T) == typeof(short))
        {
            NumberByteHelper.FormatInt16(buffer, 0, Size, (short)(int)(object)value, padding, zerofill, filler);
        }
        else if (typeof(T) == typeof(long))
        {
            NumberByteHelper.FormatInt64(buffer, 0, Size, (long)(object)value, padding, zerofill, filler);
        }
    }
}
