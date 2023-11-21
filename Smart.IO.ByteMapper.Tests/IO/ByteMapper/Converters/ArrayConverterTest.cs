namespace Smart.IO.ByteMapper.Converters;

using Smart.IO.ByteMapper.Mock;

using Xunit;

public class ArrayConverterTest
{
    private const int Offset = 1;

    private static readonly int[] Value = { 1, 1, 1 };

    private static readonly int[] ValueShortage = { 2, 2 };

    private static readonly byte[] ValueBytes = TestBytes.Offset(
        Offset,
        new byte[] { 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01 });

    private static readonly byte[] ValueShortageBytes = TestBytes.Offset(
        Offset,
        new byte[] { 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00 });

    private static readonly byte[] NullBytes = TestBytes.Offset(
        Offset,
        new byte[12]);

    private readonly ArrayConverter converter = new(
        x => new int[x],
        3,
        0x00,
        4,
        new BigEndianIntBinaryConverter());

    [Fact]
    public void ReadToIntArray()
    {
        Assert.Equal(Value, (int[])converter.Read(ValueBytes, Offset));
    }

    [Fact]
    public void WriteIntArrayToBuffer()
    {
        var buffer = new byte[12 + Offset];

        // Value
        converter.Write(buffer, Offset, Value);
        Assert.Equal(ValueBytes, buffer);

        // ValueShortage
        converter.Write(buffer, Offset, ValueShortage);
        Assert.Equal(ValueShortageBytes, buffer);

        // Null
        converter.Write(buffer, Offset, null);
        Assert.Equal(NullBytes, buffer);
    }
}
