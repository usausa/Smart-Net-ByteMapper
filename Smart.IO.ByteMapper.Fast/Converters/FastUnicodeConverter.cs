namespace Smart.IO.ByteMapper.Converters;

using System.Runtime.InteropServices;

// UTF-16LE（.NET 内部表現）文字列コンバーター。
public sealed class FastUnicodeConverter
{
    private readonly bool trim;
    private readonly Padding padding;
    private readonly char filler;

    public int Size { get; }

    public FastUnicodeConverter(int length, bool trim = true, Padding padding = Padding.Right, char filler = ' ')
    {
        Size = length;
        this.trim = trim;
        this.padding = padding;
        this.filler = filler;
    }

    public string Read(ReadOnlySpan<byte> buffer)
    {
        var chars = MemoryMarshal.Cast<byte, char>(buffer[..Size]);
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
        var destination = buffer[..Size];
        if (String.IsNullOrEmpty(value))
        {
            MemoryMarshal.Cast<byte, char>(destination).Fill(filler);
            return;
        }

        var size = value.Length * 2;
        if (size >= Size)
        {
            MemoryMarshal.Cast<char, byte>(value.AsSpan(0, Size / 2)).CopyTo(destination);
        }
        else if (padding == Padding.Right)
        {
            MemoryMarshal.Cast<char, byte>(value.AsSpan()).CopyTo(destination[..size]);
            MemoryMarshal.Cast<byte, char>(destination[size..]).Fill(filler);
        }
        else
        {
            var pad = Size - size;
            MemoryMarshal.Cast<byte, char>(destination[..pad]).Fill(filler);
            MemoryMarshal.Cast<char, byte>(value.AsSpan()).CopyTo(destination[pad..]);
        }
    }
}
