namespace Smart.IO.ByteMapper.AspNetCore;

using System.Buffers;

internal static class StreamExtensions
{
    // Reads exactly `count` bytes from `stream` into `buffer[0..count]`.
    // Returns true when all bytes were read, false when EOF is reached before `count` bytes.
    internal static async ValueTask<bool> ReadExactAsync(
        this Stream stream,
        byte[] buffer,
        int count,
        CancellationToken cancellationToken)
    {
        var totalRead = 0;
        while (totalRead < count)
        {
            var read = await stream.ReadAsync(buffer.AsMemory(totalRead, count - totalRead), cancellationToken).ConfigureAwait(false);
            if (read == 0)
            {
                return false;
            }
            totalRead += read;
        }
        return true;
    }

    // Reads fixed-length records of `elementSize` bytes from `stream`, invoking `onElement` for each
    // complete record. A single ReadAsync fills a `bufferSize` chunk so multiple records are parsed per
    // I/O call; an element straddling a chunk boundary is carried over to the next read. A trailing
    // partial element at EOF is discarded (treated as incomplete data).
    // `state` is threaded to `onElement` so callers can pass a static (closure-free) callback.
    internal static async ValueTask ReadRecordsAsync<TState>(
        this Stream stream,
        int elementSize,
        int bufferSize,
        TState state,
        Action<TState, ReadOnlyMemory<byte>> onElement,
        CancellationToken cancellationToken)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(Math.Max(bufferSize, elementSize));
        try
        {
            var filled = 0;
            int read;
            while ((read = await stream.ReadAsync(buffer.AsMemory(filled, buffer.Length - filled), cancellationToken).ConfigureAwait(false)) > 0)
            {
                filled += read;

                var offset = 0;
                while (offset + elementSize <= filled)
                {
                    onElement(state, buffer.AsMemory(offset, elementSize));
                    offset += elementSize;
                }

                // Carry the incomplete trailing element to the front of the buffer.
                var leftover = filled - offset;
                if (offset > 0 && leftover > 0)
                {
                    buffer.AsSpan(offset, leftover).CopyTo(buffer.AsSpan(0, leftover));
                }
                filled = leftover;
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}
