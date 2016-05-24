namespace Smart.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public static class FileHelper
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="blockSize"></param>
        /// <returns></returns>
        public static IEnumerable<byte[]> ReadAllByteBlocks(string path, int blockSize)
        {
            return ReadAllByteBlocks(path, blockSize, 0);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="blockSize"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static IEnumerable<byte[]> ReadAllByteBlocks(string path, int blockSize, int offset)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var buffer = new byte[blockSize];
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                fs.Position = offset;

                while (fs.Read(buffer, 0, blockSize) == blockSize)
                {
                    var bytes = new byte[blockSize];
                    Buffer.BlockCopy(buffer, 0, bytes, 0, blockSize);
                    yield return bytes;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="contents"></param>
        public static void WriteAllByteBlocks(string path, IEnumerable<byte[]> contents)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (contents == null)
            {
                throw new ArgumentNullException(nameof(contents));
            }

            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                foreach (var bytes in contents)
                {
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
        }
    }
}
