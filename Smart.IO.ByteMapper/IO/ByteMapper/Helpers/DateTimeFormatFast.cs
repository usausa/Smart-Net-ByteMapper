#nullable enable annotations
namespace Smart.IO.ByteMapper.Helpers;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[Flags]
internal enum DateTimeFormatBlocks : byte
{
    Date = 1,
    Time = 2,
    Offset = 4
}

// Block-based fast-path for compact, fully-specified DateTime-family <-> ASCII byte conversions.
// Supports formats composed exclusively of: yyyy/yy, MM, dd, HH, mm, ss, zzz, and ASCII literals.
// TryCreate returns null when the format contains unsupported specifiers or missing required blocks.
internal sealed class DateTimeFormatFast
{
    private const int MaxWidth = 64;

    private static readonly ushort[] LutUtf8 = BuildLutUtf8();

    private enum BlockKind : byte
    {
        Year4,
        Year2,
        Month,
        Day,
        Hour,
        Minute,
        Second,
        Literal,
        OffsetZzz // ±HH:MM, 6 bytes fixed
    }

    private readonly struct Block
    {
        public readonly BlockKind Kind;
        public readonly byte Offset;
        public readonly byte LiteralByte;

        public Block(BlockKind kind, byte offset, byte literalByte = 0)
        {
            Kind = kind;
            Offset = offset;
            LiteralByte = literalByte;
        }
    }

    private readonly Block[] blocks;

    private readonly bool hasOffsetBlock;

    public int Width { get; }

    private DateTimeFormatFast(Block[] blocks, int width, bool hasOffsetBlock)
    {
        this.blocks = blocks;
        Width = width;
        this.hasOffsetBlock = hasOffsetBlock;
    }

    public static DateTimeFormatFast? TryCreate(string format, DateTimeFormatBlocks allowed)
    {
        if (string.IsNullOrEmpty(format))
        {
            return null;
        }

        var list = new List<Block>(format.Length);
        var offset = 0;
        var i = 0;
        var hasYear = false;
        var hasMonth = false;
        var hasDay = false;
        var hasHour = false;
        var hasMinute = false;
        var hasSecond = false;
        var hasOffset = false;

        while (i < format.Length)
        {
            var c = format[i];
            switch (c)
            {
                case 'y':
                {
                    if ((allowed & DateTimeFormatBlocks.Date) == 0)
                    {
                        return null;
                    }

                    var run = CountRun(format, i, 'y');
                    if (run == 4)
                    {
                        list.Add(new Block(BlockKind.Year4, (byte)offset));
                        offset += 4;
                    }
                    else if (run == 2)
                    {
                        list.Add(new Block(BlockKind.Year2, (byte)offset));
                        offset += 2;
                    }
                    else
                    {
                        return null;
                    }

                    hasYear = true;
                    i += run;
                    break;
                }

                case 'M':
                {
                    if ((allowed & DateTimeFormatBlocks.Date) == 0)
                    {
                        return null;
                    }

                    var run = CountRun(format, i, 'M');
                    if (run != 2)
                    {
                        return null;
                    }

                    list.Add(new Block(BlockKind.Month, (byte)offset));
                    offset += 2;
                    hasMonth = true;
                    i += run;
                    break;
                }

                case 'd':
                {
                    if ((allowed & DateTimeFormatBlocks.Date) == 0)
                    {
                        return null;
                    }

                    var run = CountRun(format, i, 'd');
                    if (run != 2)
                    {
                        return null;
                    }

                    list.Add(new Block(BlockKind.Day, (byte)offset));
                    offset += 2;
                    hasDay = true;
                    i += run;
                    break;
                }

                case 'H':
                {
                    if ((allowed & DateTimeFormatBlocks.Time) == 0)
                    {
                        return null;
                    }

                    var run = CountRun(format, i, 'H');
                    if (run != 2)
                    {
                        return null;
                    }

                    list.Add(new Block(BlockKind.Hour, (byte)offset));
                    offset += 2;
                    hasHour = true;
                    i += run;
                    break;
                }

                case 'm':
                {
                    if ((allowed & DateTimeFormatBlocks.Time) == 0)
                    {
                        return null;
                    }

                    var run = CountRun(format, i, 'm');
                    if (run != 2)
                    {
                        return null;
                    }

                    list.Add(new Block(BlockKind.Minute, (byte)offset));
                    offset += 2;
                    hasMinute = true;
                    i += run;
                    break;
                }

                case 's':
                {
                    if ((allowed & DateTimeFormatBlocks.Time) == 0)
                    {
                        return null;
                    }

                    var run = CountRun(format, i, 's');
                    if (run != 2)
                    {
                        return null;
                    }

                    list.Add(new Block(BlockKind.Second, (byte)offset));
                    offset += 2;
                    hasSecond = true;
                    i += run;
                    break;
                }

                case 'z':
                {
                    if ((allowed & DateTimeFormatBlocks.Offset) == 0)
                    {
                        return null;
                    }

                    var run = CountRun(format, i, 'z');
                    if (run != 3)
                    {
                        return null;
                    }

                    list.Add(new Block(BlockKind.OffsetZzz, (byte)offset));
                    offset += 6;
                    hasOffset = true;
                    i += run;
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

        // Date components are required when Date is in allowed
        if ((allowed & DateTimeFormatBlocks.Date) != 0 && (!hasYear || !hasMonth || !hasDay))
        {
            return null;
        }

        // Time components are required when Time is the only group (e.g. TimeOnly)
        if ((allowed & DateTimeFormatBlocks.Time) != 0 && (allowed & DateTimeFormatBlocks.Date) == 0
            && (!hasHour || !hasMinute || !hasSecond))
        {
            return null;
        }

        return new DateTimeFormatFast(list.ToArray(), offset, hasOffset);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsReservedSpecifier(char c)
    {
        return c is 'y' or 'M' or 'd' or 'H' or 'm' or 's'
            or 'f' or 'F' or 'g' or 'G' or 'h' or 't' or 'z' or 'K'
            or '\\' or '"' or '\'';
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

    // --- TryFormat overloads ---

    public bool TryFormat(Span<byte> destination, DateTime value)
        => TryFormatCore(destination, value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);

    public bool TryFormat(Span<byte> destination, DateTimeOffset value)
        => TryFormatCore(destination, value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, (int)value.Offset.TotalMinutes);

    public bool TryFormat(Span<byte> destination, DateOnly value)
        => TryFormatCore(destination, value.Year, value.Month, value.Day, 0, 0, 0);

    public bool TryFormat(Span<byte> destination, TimeOnly value)
        => TryFormatCore(destination, 1, 1, 1, value.Hour, value.Minute, value.Second);

    private bool TryFormatCore(Span<byte> destination, int year, int month, int day, int hour, int minute, int second, int offsetMinutes = 0)
    {
        if (destination.Length < Width)
        {
            return false;
        }

        ref var dst = ref MemoryMarshal.GetReference(destination);
        var lut = LutUtf8;

        foreach (var b in blocks)
        {
            switch (b.Kind)
            {
                case BlockKind.Year4:
                {
                    var hi = year / 100;
                    var lo = year - (hi * 100);
                    WriteTwoBytes(ref dst, b.Offset, lut[hi]);
                    WriteTwoBytes(ref dst, b.Offset + 2, lut[lo]);
                    break;
                }

                case BlockKind.Year2:
                    WriteTwoBytes(ref dst, b.Offset, lut[year % 100]);
                    break;

                case BlockKind.Month:
                    WriteTwoBytes(ref dst, b.Offset, lut[month]);
                    break;

                case BlockKind.Day:
                    WriteTwoBytes(ref dst, b.Offset, lut[day]);
                    break;

                case BlockKind.Hour:
                    WriteTwoBytes(ref dst, b.Offset, lut[hour]);
                    break;

                case BlockKind.Minute:
                    WriteTwoBytes(ref dst, b.Offset, lut[minute]);
                    break;

                case BlockKind.Second:
                    WriteTwoBytes(ref dst, b.Offset, lut[second]);
                    break;

                case BlockKind.Literal:
                    Unsafe.Add(ref dst, b.Offset) = b.LiteralByte;
                    break;

                case BlockKind.OffsetZzz:
                {
                    var absMin = offsetMinutes < 0 ? -offsetMinutes : offsetMinutes;
                    Unsafe.Add(ref dst, b.Offset) = offsetMinutes < 0 ? (byte)'-' : (byte)'+';
                    WriteTwoBytes(ref dst, b.Offset + 1, lut[absMin / 60]);
                    Unsafe.Add(ref dst, b.Offset + 3) = (byte)':';
                    WriteTwoBytes(ref dst, b.Offset + 4, lut[absMin % 60]);
                    break;
                }
            }
        }

        return true;
    }

    // --- TryParse overloads ---

    public bool TryParse(ReadOnlySpan<byte> source, out DateTime value)
    {
        if (!TryParseCore(source, out var y, out var mo, out var d, out var h, out var mi, out var s, out _)
            || (d > DaysInMonth(y, mo)))
        {
            value = default;
            return false;
        }

        value = new DateTime(y, mo, d, h, mi, s);
        return true;
    }

    public bool TryParse(ReadOnlySpan<byte> source, out DateTimeOffset value)
    {
        if (!TryParseCore(source, out var y, out var mo, out var d, out var h, out var mi, out var s, out var offsetMin)
            || (d > DaysInMonth(y, mo)))
        {
            value = default;
            return false;
        }

        // When format contains zzz, use the parsed offset exactly.
        // Otherwise use local timezone (DateTimeKind.Unspecified → local offset),
        // matching BCL DateTimeOffset.TryParseExact behaviour for formats without an offset specifier.
        value = hasOffsetBlock
            ? new DateTimeOffset(y, mo, d, h, mi, s, TimeSpan.FromMinutes(offsetMin))
            : new DateTimeOffset(new DateTime(y, mo, d, h, mi, s));
        return true;
    }

    public bool TryParse(ReadOnlySpan<byte> source, out DateOnly value)
    {
        if (!TryParseCore(source, out var y, out var mo, out var d, out _, out _, out _, out _)
            || (d > DaysInMonth(y, mo)))
        {
            value = default;
            return false;
        }

        value = new DateOnly(y, mo, d);
        return true;
    }

    public bool TryParse(ReadOnlySpan<byte> source, out TimeOnly value)
    {
        if (!TryParseCore(source, out _, out _, out _, out var h, out var mi, out var s, out _))
        {
            value = default;
            return false;
        }

        value = new TimeOnly(h, mi, s);
        return true;
    }

    private bool TryParseCore(
        ReadOnlySpan<byte> source,
        out int year,
        out int month,
        out int day,
        out int hour,
        out int minute,
        out int second,
        out int offsetMinutes)
    {
        year = 1;
        month = 1;
        day = 1;
        hour = 0;
        minute = 0;
        second = 0;
        offsetMinutes = 0;

        if (source.Length < Width)
        {
            return false;
        }

        foreach (var b in blocks)
        {
            switch (b.Kind)
            {
                case BlockKind.Year4:
                    if (!TryRead4(source, b.Offset, out year) || (year < 1))
                    {
                        return false;
                    }

                    break;

                case BlockKind.Year2:
                {
                    if (!TryRead2(source, b.Offset, out var yy))
                    {
                        return false;
                    }

                    year = yy <= 29 ? 2000 + yy : 1900 + yy;
                    break;
                }

                case BlockKind.Month:
                    if (!TryRead2(source, b.Offset, out month) || (month < 1) || (month > 12))
                    {
                        return false;
                    }

                    break;

                case BlockKind.Day:
                    if (!TryRead2(source, b.Offset, out day) || (day < 1) || (day > 31))
                    {
                        return false;
                    }

                    break;

                case BlockKind.Hour:
                    if (!TryRead2(source, b.Offset, out hour) || (hour > 23))
                    {
                        return false;
                    }

                    break;

                case BlockKind.Minute:
                    if (!TryRead2(source, b.Offset, out minute) || (minute > 59))
                    {
                        return false;
                    }

                    break;

                case BlockKind.Second:
                    if (!TryRead2(source, b.Offset, out second) || (second > 59))
                    {
                        return false;
                    }

                    break;

                case BlockKind.Literal:
                    if (source[b.Offset] != b.LiteralByte)
                    {
                        return false;
                    }

                    break;

                case BlockKind.OffsetZzz:
                {
                    var sign = source[b.Offset];
                    if ((sign != '+') && (sign != '-'))
                    {
                        return false;
                    }

                    if (!TryRead2(source, b.Offset + 1, out var oh) || (oh > 14))
                    {
                        return false;
                    }

                    if (source[b.Offset + 3] != ':')
                    {
                        return false;
                    }

                    if (!TryRead2(source, b.Offset + 4, out var om) || (om > 59))
                    {
                        return false;
                    }

                    offsetMinutes = (oh * 60) + om;
                    if (sign == '-')
                    {
                        offsetMinutes = -offsetMinutes;
                    }

                    break;
                }
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteTwoBytes(ref byte dst, int offset, ushort pair)
    {
        Unsafe.WriteUnaligned(ref Unsafe.Add(ref dst, offset), pair);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryRead2(ReadOnlySpan<byte> src, int offset, out int value)
    {
        var d0 = (uint)(src[offset] - '0');
        var d1 = (uint)(src[offset + 1] - '0');
        if ((d0 > 9) || (d1 > 9))
        {
            value = 0;
            return false;
        }

        value = (int)((d0 * 10) + d1);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryRead4(ReadOnlySpan<byte> src, int offset, out int value)
    {
        var d0 = (uint)(src[offset] - '0');
        var d1 = (uint)(src[offset + 1] - '0');
        var d2 = (uint)(src[offset + 2] - '0');
        var d3 = (uint)(src[offset + 3] - '0');
        if ((d0 > 9) || (d1 > 9) || (d2 > 9) || (d3 > 9))
        {
            value = 0;
            return false;
        }

        value = (int)((d0 * 1000) + (d1 * 100) + (d2 * 10) + d3);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsLeapYear(int year)
        => ((year & 3) == 0) && (((year % 100) != 0) || ((year % 400) == 0));

    private static int DaysInMonth(int year, int month)
    {
        return month switch
        {
            2 => IsLeapYear(year) ? 29 : 28,
            4 or 6 or 9 or 11 => 30,
            _ => 31
        };
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
