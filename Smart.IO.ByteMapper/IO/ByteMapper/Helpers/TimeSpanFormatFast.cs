#nullable enable annotations
namespace Smart.IO.ByteMapper.Helpers;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Block-based fast-path for fixed-width TimeSpan <-> ASCII byte conversions.
// Supports: d-dddddddd (days), h/hh (hours), m/mm (minutes), s/ss (seconds),
//           f-fffffff (fractional seconds), \c (escaped literal), ASCII literals.
// TryCreate returns null for variable-width specifiers (F, g, G, single d/h/m/s beyond bounds)
// or unsupported specifiers. TryFormat returns false for negative values or component overflow.
internal sealed class TimeSpanFormatFast
{
    private const int MaxWidth = 64;

    private static readonly ushort[] LutUtf8 = BuildLutUtf8();

    // 10^n: used to validate that a value fits in n digits
    private static readonly int[] Pow10 = [1, 10, 100, 1_000, 10_000, 100_000, 1_000_000, 10_000_000, 100_000_000];

    // Ticks per digit count for fractional seconds: index = n-1, value = 10^(7-n)
    private static readonly int[] FractionDivisor = [1_000_000, 100_000, 10_000, 1_000, 100, 10, 1];

    private enum BlockKind : byte
    {
        // Data = digit count (1-8)
        Days,
        // Data = digit count (1 or 2)
        Hour,
        // Data = digit count (1 or 2)
        Minute,
        // Data = digit count (1 or 2)
        Second,
        // Data = digit count (1-7)
        Fraction,
        // Data = literal byte
        Literal
    }

    private readonly struct Block
    {
        public readonly BlockKind Kind;
        public readonly byte Offset;
        public readonly byte Data;

        public Block(BlockKind kind, byte offset, byte data = 0)
        {
            Kind = kind;
            Offset = offset;
            Data = data;
        }
    }

    private readonly Block[] blocks;

    public int Width { get; }

    private TimeSpanFormatFast(Block[] blocks, int width)
    {
        this.blocks = blocks;
        Width = width;
    }

    public static TimeSpanFormatFast? TryCreate(string format)
    {
        if (string.IsNullOrEmpty(format))
        {
            return null;
        }

        var list = new List<Block>(format.Length);
        var offset = 0;
        var i = 0;
        var hasHour = false;
        var hasMinute = false;
        var hasSecond = false;

        while (i < format.Length)
        {
            var c = format[i];
            switch (c)
            {
                case 'd':
                {
                    var run = CountRun(format, i, 'd');
                    if (run > 8)
                    {
                        return null;
                    }

                    list.Add(new Block(BlockKind.Days, (byte)offset, (byte)run));
                    offset += run;
                    i += run;
                    break;
                }

                case 'h':
                {
                    var run = CountRun(format, i, 'h');
                    if (run > 2)
                    {
                        return null;
                    }

                    list.Add(new Block(BlockKind.Hour, (byte)offset, (byte)run));
                    offset += run;
                    hasHour = true;
                    i += run;
                    break;
                }

                case 'm':
                {
                    var run = CountRun(format, i, 'm');
                    if (run > 2)
                    {
                        return null;
                    }

                    list.Add(new Block(BlockKind.Minute, (byte)offset, (byte)run));
                    offset += run;
                    hasMinute = true;
                    i += run;
                    break;
                }

                case 's':
                {
                    var run = CountRun(format, i, 's');
                    if (run > 2)
                    {
                        return null;
                    }

                    list.Add(new Block(BlockKind.Second, (byte)offset, (byte)run));
                    offset += run;
                    hasSecond = true;
                    i += run;
                    break;
                }

                case 'f':
                {
                    var run = CountRun(format, i, 'f');
                    if (run > 7)
                    {
                        return null;
                    }

                    list.Add(new Block(BlockKind.Fraction, (byte)offset, (byte)run));
                    offset += run;
                    i += run;
                    break;
                }

                case '\\':
                {
                    if ((i + 1) >= format.Length)
                    {
                        return null;
                    }

                    var next = format[i + 1];
                    if (next > 127)
                    {
                        return null;
                    }

                    list.Add(new Block(BlockKind.Literal, (byte)offset, (byte)next));
                    offset += 1;
                    i += 2;
                    break;
                }

                default:
                    if (IsReservedSpecifier(c) || (c > 127))
                    {
                        return null;
                    }

                    list.Add(new Block(BlockKind.Literal, (byte)offset, (byte)c));
                    offset += 1;
                    i++;
                    break;
            }

            if (offset > MaxWidth)
            {
                return null;
            }
        }

        if (!hasHour || !hasMinute || !hasSecond)
        {
            return null;
        }

        return new TimeSpanFormatFast(list.ToArray(), offset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsReservedSpecifier(char c)
    {
        return c is 'F' or 'g' or 'G' or 'H' or 'y' or 'M' or 'z' or 'K' or 't' or '%' or '\'' or '"';
    }

    private static int CountRun(string s, int start, char c)
    {
        var n = 0;
        while (((start + n) < s.Length) && (s[start + n] == c))
        {
            n++;
        }

        return n;
    }

    // Returns false for negative values or when a component exceeds its allocated digit count.
    public bool TryFormat(Span<byte> destination, TimeSpan value)
    {
        if (value.Ticks < 0)
        {
            return false;
        }

        if (destination.Length < Width)
        {
            return false;
        }

        var totalTicks = value.Ticks;
        var days = (int)(totalTicks / TimeSpan.TicksPerDay);
        var remaining = totalTicks % TimeSpan.TicksPerDay;
        var hours = (int)(remaining / TimeSpan.TicksPerHour);
        remaining %= TimeSpan.TicksPerHour;
        var minutes = (int)(remaining / TimeSpan.TicksPerMinute);
        remaining %= TimeSpan.TicksPerMinute;
        var seconds = (int)(remaining / TimeSpan.TicksPerSecond);
        var fractionTicks = (int)(remaining % TimeSpan.TicksPerSecond);

        ref var dst = ref MemoryMarshal.GetReference(destination);
        var lut = LutUtf8;

        foreach (var b in blocks)
        {
            switch (b.Kind)
            {
                case BlockKind.Days:
                    if (days >= Pow10[b.Data])
                    {
                        return false;
                    }

                    WriteDigits(ref dst, b.Offset, days, b.Data, lut);
                    break;

                case BlockKind.Hour:
                    if (hours > (b.Data == 1 ? 9 : 23))
                    {
                        return false;
                    }

                    WriteDigits(ref dst, b.Offset, hours, b.Data, lut);
                    break;

                case BlockKind.Minute:
                    if (minutes > (b.Data == 1 ? 9 : 59))
                    {
                        return false;
                    }

                    WriteDigits(ref dst, b.Offset, minutes, b.Data, lut);
                    break;

                case BlockKind.Second:
                    if (seconds > (b.Data == 1 ? 9 : 59))
                    {
                        return false;
                    }

                    WriteDigits(ref dst, b.Offset, seconds, b.Data, lut);
                    break;

                case BlockKind.Fraction:
                    WriteDigits(ref dst, b.Offset, fractionTicks / FractionDivisor[b.Data - 1], b.Data, lut);
                    break;

                case BlockKind.Literal:
                    Unsafe.Add(ref dst, b.Offset) = b.Data;
                    break;
            }
        }

        return true;
    }

    public bool TryParse(ReadOnlySpan<byte> source, out TimeSpan value)
    {
        value = default;

        if (source.Length < Width)
        {
            return false;
        }

        var days = 0;
        var hours = 0;
        var minutes = 0;
        var seconds = 0;
        var fractionTicks = 0L;

        foreach (var b in blocks)
        {
            switch (b.Kind)
            {
                case BlockKind.Days:
                    if (!TryReadN(source, b.Offset, b.Data, out days))
                    {
                        return false;
                    }

                    break;

                case BlockKind.Hour:
                    if (!TryReadN(source, b.Offset, b.Data, out hours) || (hours > (b.Data == 1 ? 9 : 23)))
                    {
                        return false;
                    }

                    break;

                case BlockKind.Minute:
                    if (!TryReadN(source, b.Offset, b.Data, out minutes) || (minutes > (b.Data == 1 ? 9 : 59)))
                    {
                        return false;
                    }

                    break;

                case BlockKind.Second:
                    if (!TryReadN(source, b.Offset, b.Data, out seconds) || (seconds > (b.Data == 1 ? 9 : 59)))
                    {
                        return false;
                    }

                    break;

                case BlockKind.Fraction:
                    if (!TryReadN(source, b.Offset, b.Data, out var fv))
                    {
                        return false;
                    }

                    fractionTicks = (long)fv * FractionDivisor[b.Data - 1];
                    break;

                case BlockKind.Literal:
                    if (source[b.Offset] != b.Data)
                    {
                        return false;
                    }

                    break;
            }
        }

        value = new TimeSpan(
            (days * TimeSpan.TicksPerDay) +
            (hours * TimeSpan.TicksPerHour) +
            (minutes * TimeSpan.TicksPerMinute) +
            (seconds * TimeSpan.TicksPerSecond) +
            fractionTicks);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteTwoBytes(ref byte dst, int offset, ushort pair)
    {
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref dst, offset), pair);
    }

    private static void WriteDigits(ref byte dst, int offset, int value, int n, ushort[] lut)
    {
        switch (n)
        {
            case 1:
                Unsafe.Add(ref dst, offset) = (byte)('0' + value);
                break;
            case 2:
                WriteTwoBytes(ref dst, offset, lut[value]);
                break;
            case 3:
                Unsafe.Add(ref dst, offset) = (byte)('0' + (value / 100));
                WriteTwoBytes(ref dst, offset + 1, lut[value % 100]);
                break;
            case 4:
                WriteTwoBytes(ref dst, offset, lut[value / 100]);
                WriteTwoBytes(ref dst, offset + 2, lut[value % 100]);
                break;
            case 5:
                Unsafe.Add(ref dst, offset) = (byte)('0' + (value / 10_000));
                WriteTwoBytes(ref dst, offset + 1, lut[(value / 100) % 100]);
                WriteTwoBytes(ref dst, offset + 3, lut[value % 100]);
                break;
            case 6:
                WriteTwoBytes(ref dst, offset, lut[value / 10_000]);
                WriteTwoBytes(ref dst, offset + 2, lut[(value / 100) % 100]);
                WriteTwoBytes(ref dst, offset + 4, lut[value % 100]);
                break;
            case 7:
                Unsafe.Add(ref dst, offset) = (byte)('0' + (value / 1_000_000));
                WriteTwoBytes(ref dst, offset + 1, lut[(value / 10_000) % 100]);
                WriteTwoBytes(ref dst, offset + 3, lut[(value / 100) % 100]);
                WriteTwoBytes(ref dst, offset + 5, lut[value % 100]);
                break;
            case 8:
                WriteTwoBytes(ref dst, offset, lut[value / 1_000_000]);
                WriteTwoBytes(ref dst, offset + 2, lut[(value / 10_000) % 100]);
                WriteTwoBytes(ref dst, offset + 4, lut[(value / 100) % 100]);
                WriteTwoBytes(ref dst, offset + 6, lut[value % 100]);
                break;
        }
    }

    private static bool TryReadN(ReadOnlySpan<byte> src, int offset, int n, out int value)
    {
        value = 0;
        for (var j = 0; j < n; j++)
        {
            var d = (uint)(src[offset + j] - '0');
            if (d > 9)
            {
                return false;
            }

            value = (value * 10) + (int)d;
        }

        return true;
    }

    private static ushort[] BuildLutUtf8()
    {
        var lut = new ushort[100];
        for (var v = 0; v < 100; v++)
        {
            var hi = (byte)('0' + (v / 10));
            var lo = (byte)('0' + (v % 10));
            lut[v] = (ushort)(hi | (lo << 8));
        }

        return lut;
    }
}
