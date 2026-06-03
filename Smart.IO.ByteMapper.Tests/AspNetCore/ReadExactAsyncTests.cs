namespace Smart.IO.ByteMapper.AspNetCore;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

// Stream that returns at most `chunkSize` bytes per ReadAsync call regardless of request size.
// Simulates HTTP streams that may return short reads.
internal sealed class ChunkedStream(byte[] data, int chunkSize) : Stream
{
    private int position;

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override void Flush()
    {
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (position >= data.Length)
        {
            return ValueTask.FromResult(0);
        }

        var available = Math.Min(chunkSize, data.Length - position);
        var toRead = Math.Min(available, buffer.Length);
        data.AsSpan(position, toRead).CopyTo(buffer.Span);
        position += toRead;
        return ValueTask.FromResult(toRead);
    }
}

public class ReadExactAsyncTests
{
    private static byte[] MakeData(int size)
    {
        var data = new byte[size];
        for (var i = 0; i < size; i++)
        {
            data[i] = (byte)(i & 0xFF);
        }
        return data;
    }

    [Fact]
    public async Task WhenAllBytesAvailableThenReturnsTrueAndFillsBuffer()
    {
        var data = MakeData(16);
        using var stream = new MemoryStream(data);
        var buffer = new byte[16];

        var result = await stream.ReadExactAsync(buffer, 16, CancellationToken.None);

        Assert.True(result);
        Assert.Equal(data, buffer);
    }

    [Fact]
    public async Task WhenStreamReturnsSingleByteThenAllBytesRead()
    {
        var data = MakeData(16);
        using var stream = new ChunkedStream(data, chunkSize: 1);
        var buffer = new byte[16];

        var result = await stream.ReadExactAsync(buffer, 16, CancellationToken.None);

        Assert.True(result);
        Assert.Equal(data, buffer);
    }

    [Fact]
    public async Task WhenEofBeforeCountThenReturnsFalse()
    {
        var data = MakeData(8);
        using var stream = new MemoryStream(data);
        var buffer = new byte[16];

        var result = await stream.ReadExactAsync(buffer, 16, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task WhenEmptyStreamThenReturnsFalse()
    {
        using var stream = new MemoryStream([]);
        var buffer = new byte[4];

        var result = await stream.ReadExactAsync(buffer, 4, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task WhenReadingSubsetOfBufferThenCorrectBytesRead()
    {
        var data = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };
        using var stream = new ChunkedStream(data, chunkSize: 1);
        var buffer = new byte[8];

        var result = await stream.ReadExactAsync(buffer, 4, CancellationToken.None);

        Assert.True(result);
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03, 0x04 }, buffer[..4]);
    }
}
