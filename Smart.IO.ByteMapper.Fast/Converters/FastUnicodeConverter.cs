namespace Smart.IO.ByteMapper.Converters;

using System.Runtime.InteropServices;

// UTF-16LE（.NET 内部表現）文字列コンバーター。
#pragma warning disable IDE0032
public sealed class FastUnicodeConverter
{
    private readonly int size;
    private readonly bool trim;
    private readonly Padding padding;
    private readonly char filler;

    public int Size => size;

    public FastUnicodeConverter(int length, bool trim = true, Padding padding = Padding.Right, char filler = ' ')
    {
        size = length;
        this.trim = trim;
        this.padding = padding;
        this.filler = filler;
    }

    public string Read(ReadOnlySpan<byte> buffer)
    {
        var chars = MemoryMarshal.Cast<byte, char>(buffer[..size]);
        if (trim)
        {
            if (padding == Padding.Left)
            {
                var idx = chars.IndexOfAnyExcept(filler);
                if (idx < 0)
                {
                    return string.Empty;
                }

                chars = chars[idx..];
            }
            else
            {
                var idx = chars.LastIndexOfAnyExcept(filler);
                if (idx < 0)
                {
                    return string.Empty;
                }

                chars = chars[..(idx + 1)];
            }
        }

        return chars.Length == 0 ? string.Empty : new string(chars);
    }

    public void Write(Span<byte> buffer, string? value)
    {
        var destination = buffer[..size];
        if (String.IsNullOrEmpty(value))
        {
            MemoryMarshal.Cast<byte, char>(destination).Fill(filler);
            return;
        }

        var byteLen = value.Length * 2;
        if (byteLen >= size)
        {
            MemoryMarshal.Cast<char, byte>(value.AsSpan(0, size / 2)).CopyTo(destination);
        }
        else if (padding == Padding.Right)
        {
            MemoryMarshal.Cast<char, byte>(value.AsSpan()).CopyTo(destination[..byteLen]);
            MemoryMarshal.Cast<byte, char>(destination[byteLen..]).Fill(filler);
        }
        else
        {
            var pad = size - byteLen;
            MemoryMarshal.Cast<byte, char>(destination[..pad]).Fill(filler);
            MemoryMarshal.Cast<char, byte>(value.AsSpan()).CopyTo(destination[pad..]);
        }
    }
}
#pragma warning restore IDE0032
