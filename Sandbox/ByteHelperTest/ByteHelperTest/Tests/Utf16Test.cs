namespace ByteHelperTest.Tests;

using System.Text;

using Xunit;

public class Utf16Test
{
    [Fact]
    public void Convert()
    {
        var buffer = new byte[30];
        ByteHelper3.CopyUtf16Bytes("あいうえお12345", buffer, 0, buffer.Length, Padding.Right, ' ');

        Assert.Equal("あいうえお12345     ", Encoding.Unicode.GetString(buffer));

        var str = ByteHelper3.GetUtf16String(buffer, 0, buffer.Length, Padding.Right, ' ');
        Assert.Equal("あいうえお12345", str);
    }
}
