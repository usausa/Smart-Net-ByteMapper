namespace Smart.IO.ByteMapper.Helpers;

internal sealed class FastDateTimeFormatEntry
{
    public char Part { get; }

    public int Length { get; }

    public byte[]? Bytes { get; }

    public FastDateTimeFormatEntry(char part, int length, byte[]? bytes)
    {
        Part = part;
        Length = length;
        Bytes = bytes;
    }
}
