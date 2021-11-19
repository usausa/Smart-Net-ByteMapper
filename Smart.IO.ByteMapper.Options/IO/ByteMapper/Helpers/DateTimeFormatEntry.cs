namespace Smart.IO.ByteMapper.Helpers;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Ignore")]
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
