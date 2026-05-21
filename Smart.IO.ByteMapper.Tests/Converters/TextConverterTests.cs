namespace Smart.IO.ByteMapper.Converters;

using System;
using System.Text;

using Xunit;

public class TextConverterTests
{
    [Fact]
    public void WhenWriteAndReadThenSameString()
    {
        var converter = new TextConverter(8, 20127, true, Padding.Right, 0x20);
        var buffer = new byte[8];
        converter.Write(buffer, "Hello");
        Assert.Equal("Hello", converter.Read(buffer));
    }

    [Fact]
    public void WhenWriteShortStringThenPaddedRight()
    {
        var converter = new TextConverter(8, 20127, false, Padding.Right, 0x20);
        var buffer = new byte[8];
        converter.Write(buffer, "AB");
        // "AB      " (6 spaces)
        Assert.Equal((byte)'A', buffer[0]);
        Assert.Equal((byte)'B', buffer[1]);
        for (var i = 2; i < 8; i++)
        {
            Assert.Equal(0x20, buffer[i]);
        }
    }

    [Fact]
    public void WhenWriteShortStringThenPaddedLeft()
    {
        var converter = new TextConverter(8, 20127, false, Padding.Left, 0x20);
        var buffer = new byte[8];
        converter.Write(buffer, "AB");
        // "      AB"
        for (var i = 0; i < 6; i++)
        {
            Assert.Equal(0x20, buffer[i]);
        }
        Assert.Equal((byte)'A', buffer[6]);
        Assert.Equal((byte)'B', buffer[7]);
    }

    [Fact]
    public void WhenReadWithTrimThenStripped()
    {
        var converter = new TextConverter(8, 20127, true, Padding.Right, 0x20);
        var buffer = Encoding.ASCII.GetBytes("Hello   ");
        Assert.Equal("Hello", converter.Read(buffer));
    }

    [Fact]
    public void WhenReadWithoutTrimThenNotStripped()
    {
        var converter = new TextConverter(8, 20127, false, Padding.Right, 0x20);
        var buffer = Encoding.ASCII.GetBytes("Hello   ");
        Assert.Equal("Hello   ", converter.Read(buffer));
    }

    [Fact]
    public void WhenWriteNullThenFillerFilled()
    {
        var converter = new TextConverter(4, 20127, true, Padding.Right, 0x20);
        var buffer = new byte[4];
        converter.Write(buffer, null!);
        Assert.All(buffer, b => Assert.Equal(0x20, b));
    }

    [Fact]
    public void WhenSizeMatchesLength()
    {
        var converter = new TextConverter(16, 20127, true, Padding.Right, 0x20);
        Assert.Equal(16, converter.Size);
    }
}
