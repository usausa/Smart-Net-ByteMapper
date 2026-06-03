namespace Smart.IO.ByteMapper.AspNetCore;

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
}
