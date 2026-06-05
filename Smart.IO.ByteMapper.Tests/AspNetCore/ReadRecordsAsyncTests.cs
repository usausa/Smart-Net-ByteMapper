// ReSharper disable UseUtf8StringLiteral
#pragma warning disable IDE0230
namespace Smart.IO.ByteMapper.AspNetCore;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

// Verifies the batched fixed-length record reader: multiple records per ReadAsync, short reads,
// elements straddling buffer boundaries, and trailing partial discard at EOF.
public class ReadRecordsAsyncTests
{
    private const int ElementSize = 4;

    private static byte[] MakeRecords(int count)
    {
        var data = new byte[count * ElementSize];
        for (var i = 0; i < data.Length; i++)
        {
            data[i] = (byte)(i & 0xFF);
        }
        return data;
    }

    private static async Task<List<byte[]>> ReadAllAsync(Stream stream, int bufferSize)
    {
        var items = new List<byte[]>();
        await stream.ReadRecordsAsync(
            ElementSize,
            bufferSize,
            items,
            static (list, mem) => list.Add(mem.ToArray()),
            CancellationToken.None).ConfigureAwait(false);
        return items;
    }

    [Fact]
    public async Task WhenExactMultipleRecordsThenAllParsedWithContent()
    {
        var data = MakeRecords(3);
        using var stream = new MemoryStream(data);

        var items = await ReadAllAsync(stream, bufferSize: 64);

        Assert.Equal(3, items.Count);
        Assert.Equal(new byte[] { 0, 1, 2, 3 }, items[0]);
        Assert.Equal(new byte[] { 4, 5, 6, 7 }, items[1]);
        Assert.Equal(new byte[] { 8, 9, 10, 11 }, items[2]);
    }

    [Fact]
    public async Task WhenBufferSmallerThanDataThenMultipleReadsParseAll()
    {
        var data = MakeRecords(10);
        // bufferSize 6 holds one 4-byte element + 2 leftover, forcing boundary carry-over.
        using var stream = new MemoryStream(data);

        var items = await ReadAllAsync(stream, bufferSize: 6);

        Assert.Equal(10, items.Count);
        Assert.Equal(new byte[] { 36, 37, 38, 39 }, items[9]);
    }

    [Fact]
    public async Task WhenStreamReturnsOneBytePerReadThenAllParsed()
    {
        var data = MakeRecords(7);
        await using var stream = new ChunkedStream(data, chunkSize: 1);

        var items = await ReadAllAsync(stream, bufferSize: 64);

        Assert.Equal(7, items.Count);
        Assert.Equal(new byte[] { 24, 25, 26, 27 }, items[6]);
    }

    [Fact]
    public async Task WhenChunkSizeStraddlesElementBoundaryThenAllParsed()
    {
        var data = MakeRecords(8);
        // chunkSize 3 never aligns with elementSize 4, so every element straddles reads.
        await using var stream = new ChunkedStream(data, chunkSize: 3);

        var items = await ReadAllAsync(stream, bufferSize: 16);

        Assert.Equal(8, items.Count);
        for (var i = 0; i < items.Count; i++)
        {
            Assert.Equal(data.AsSpan(i * ElementSize, ElementSize).ToArray(), items[i]);
        }
    }

    [Fact]
    public async Task WhenTrailingPartialElementThenItIsDiscarded()
    {
        // 3 full records + 2 trailing bytes (incomplete 4th).
        var data = new byte[(3 * ElementSize) + 2];
        using var stream = new MemoryStream(data);

        var items = await ReadAllAsync(stream, bufferSize: 64);

        Assert.Equal(3, items.Count);
    }

    [Fact]
    public async Task WhenEmptyStreamThenNoRecords()
    {
        using var stream = new MemoryStream([]);

        var items = await ReadAllAsync(stream, bufferSize: 64);

        Assert.Empty(items);
    }

    [Fact]
    public async Task WhenBufferSizeSmallerThanElementThenStillParses()
    {
        // bufferSize below elementSize must be clamped up to hold at least one element.
        var data = MakeRecords(4);
        using var stream = new MemoryStream(data);

        var items = await ReadAllAsync(stream, bufferSize: 1);

        Assert.Equal(4, items.Count);
    }
}
