namespace Smart.IO.ByteMapper.Helpers;

#pragma warning disable CA1819
public sealed class DateTimeFormatEntry
{
    public char Part { get; }

    public int Length { get; }

    public byte[] Bytes { get; }

    public DateTimeFormatEntry(char part, int length, byte[] bytes)
    {
        Part = part;
        Length = length;
        Bytes = bytes;
    }
}
#pragma warning restore CA1819
