namespace Smart
{
    using System;
    using System.Linq;

    /// <summary>
    ///
    /// </summary>
    public static class ArrayExtensions
    {
        //--------------------------------------------------------------------------------
        // Fill
        //--------------------------------------------------------------------------------

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static T[] Fill<T>(this T[] array, T value)
        {
            return Fill(array, 0, array.Length, value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static T[] Fill<T>(this T[] array, int offset, int length, T value)
        {
            array[offset] = value;

            int copy;
            for (copy = 1; copy <= length / 2; copy <<= 1)
            {
                Array.Copy(array, offset, array, offset + copy, copy);
            }

            Array.Copy(array, offset, array, offset + copy, length - copy);

            return array;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static byte[] Fill(this byte[] array, byte value)
        {
            return Fill(array, 0, array.Length, value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static byte[] Fill(this byte[] array, int offset, int length, byte value)
        {
            array[offset] = value;

            int copy;
            for (copy = 1; copy <= length / 2; copy <<= 1)
            {
                Buffer.BlockCopy(array, offset, array, offset + copy, copy);
            }

            Buffer.BlockCopy(array, offset, array, offset + copy, length - copy);

            return array;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static short[] Fill(this short[] array, short value)
        {
            return Fill(array, 0, array.Length, value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static short[] Fill(this short[] array, int offset, int length, short value)
        {
            array[offset] = value;

            int copy;
            for (copy = 1; copy <= length / 2; copy <<= 1)
            {
                Buffer.BlockCopy(array, offset, array, offset + copy, copy);
            }

            Buffer.BlockCopy(array, offset, array, offset + copy, length - copy);

            return array;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static int[] Fill(this int[] array, int value)
        {
            return Fill(array, 0, array.Length, value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static int[] Fill(this int[] array, int offset, int length, int value)
        {
            array[offset] = value;

            int copy;
            for (copy = 1; copy <= length / 2; copy <<= 1)
            {
                Buffer.BlockCopy(array, offset, array, offset + copy, copy);
            }

            Buffer.BlockCopy(array, offset, array, offset + copy, length - copy);

            return array;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static long[] Fill(this long[] array, long value)
        {
            return Fill(array, 0, array.Length, value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static long[] Fill(this long[] array, int offset, int length, long value)
        {
            array[offset] = value;

            int copy;
            for (copy = 1; copy <= length / 2; copy <<= 1)
            {
                Buffer.BlockCopy(array, offset, array, offset + copy, copy);
            }

            Buffer.BlockCopy(array, offset, array, offset + copy, length - copy);

            return array;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static float[] Fill(this float[] array, float value)
        {
            return Fill(array, 0, array.Length, value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static float[] Fill(this float[] array, int offset, int length, float value)
        {
            array[offset] = value;

            int copy;
            for (copy = 1; copy <= length / 2; copy <<= 1)
            {
                Buffer.BlockCopy(array, offset, array, offset + copy, copy);
            }

            Buffer.BlockCopy(array, offset, array, offset + copy, length - copy);

            return array;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static double[] Fill(this double[] array, double value)
        {
            return Fill(array, 0, array.Length, value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static double[] Fill(this double[] array, int offset, int length, double value)
        {
            array[offset] = value;

            int copy;
            for (copy = 1; copy <= length / 2; copy <<= 1)
            {
                Buffer.BlockCopy(array, offset, array, offset + copy, copy);
            }

            Buffer.BlockCopy(array, offset, array, offset + copy, length - copy);

            return array;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static bool[] Fill(this bool[] array, bool value)
        {
            return Fill(array, 0, array.Length, value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static bool[] Fill(this bool[] array, int offset, int length, bool value)
        {
            array[offset] = value;

            int copy;
            for (copy = 1; copy <= length / 2; copy <<= 1)
            {
                Buffer.BlockCopy(array, offset, array, offset + copy, copy);
            }

            Buffer.BlockCopy(array, offset, array, offset + copy, length - copy);

            return array;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static char[] Fill(this char[] array, char value)
        {
            return Fill(array, 0, array.Length, value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static char[] Fill(this char[] array, int offset, int length, char value)
        {
            array[offset] = value;

            int copy;
            for (copy = 1; copy <= length / 2; copy <<= 1)
            {
                Buffer.BlockCopy(array, offset, array, offset + copy, copy);
            }

            Buffer.BlockCopy(array, offset, array, offset + copy, length - copy);

            return array;
        }

        //--------------------------------------------------------------------------------
        // Combine
        //--------------------------------------------------------------------------------

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static T[] Combine<T>(this T[] array, params T[][] others)
        {
            if (others == null)
            {
                throw new ArgumentNullException(nameof(others));
            }

            var result = new T[array.Length + others.Sum(_ => _.Length)];

            Array.Copy(array, 0, result, 0, array.Length);

            var offset = array.Length;
            foreach (var other in others)
            {
                Array.Copy(other, 0, result, offset, other.Length);
                offset += other.Length;
            }

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static byte[] Combine(this byte[] array, params byte[][] others)
        {
            if (others == null)
            {
                throw new ArgumentNullException(nameof(others));
            }

            var result = new byte[array.Length + others.Sum(_ => _.Length)];

            Buffer.BlockCopy(array, 0, result, 0, array.Length);

            var offset = array.Length;
            foreach (var other in others)
            {
                Buffer.BlockCopy(other, 0, result, offset, other.Length);
                offset += other.Length;
            }

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static short[] Combine(this short[] array, params short[][] others)
        {
            if (others == null)
            {
                throw new ArgumentNullException(nameof(others));
            }

            var result = new short[array.Length + others.Sum(_ => _.Length)];

            Buffer.BlockCopy(array, 0, result, 0, array.Length);

            var offset = array.Length;
            foreach (var other in others)
            {
                Buffer.BlockCopy(other, 0, result, offset, other.Length);
                offset += other.Length;
            }

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static int[] Combine(this int[] array, params int[][] others)
        {
            if (others == null)
            {
                throw new ArgumentNullException(nameof(others));
            }

            var result = new int[array.Length + others.Sum(_ => _.Length)];

            Buffer.BlockCopy(array, 0, result, 0, array.Length);

            var offset = array.Length;
            foreach (var other in others)
            {
                Buffer.BlockCopy(other, 0, result, offset, other.Length);
                offset += other.Length;
            }

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static long[] Combine(this long[] array, params long[][] others)
        {
            if (others == null)
            {
                throw new ArgumentNullException(nameof(others));
            }

            var result = new long[array.Length + others.Sum(_ => _.Length)];

            Buffer.BlockCopy(array, 0, result, 0, array.Length);

            var offset = array.Length;
            foreach (var other in others)
            {
                Buffer.BlockCopy(other, 0, result, offset, other.Length);
                offset += other.Length;
            }

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static float[] Combine(this float[] array, params float[][] others)
        {
            if (others == null)
            {
                throw new ArgumentNullException(nameof(others));
            }

            var result = new float[array.Length + others.Sum(_ => _.Length)];

            Buffer.BlockCopy(array, 0, result, 0, array.Length);

            var offset = array.Length;
            foreach (var other in others)
            {
                Buffer.BlockCopy(other, 0, result, offset, other.Length);
                offset += other.Length;
            }

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static double[] Combine(this double[] array, params double[][] others)
        {
            if (others == null)
            {
                throw new ArgumentNullException(nameof(others));
            }

            var result = new double[array.Length + others.Sum(_ => _.Length)];

            Buffer.BlockCopy(array, 0, result, 0, array.Length);

            var offset = array.Length;
            foreach (var other in others)
            {
                Buffer.BlockCopy(other, 0, result, offset, other.Length);
                offset += other.Length;
            }

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static bool[] Combine(this bool[] array, params bool[][] others)
        {
            if (others == null)
            {
                throw new ArgumentNullException(nameof(others));
            }

            var result = new bool[array.Length + others.Sum(_ => _.Length)];

            Buffer.BlockCopy(array, 0, result, 0, array.Length);

            var offset = array.Length;
            foreach (var other in others)
            {
                Buffer.BlockCopy(other, 0, result, offset, other.Length);
                offset += other.Length;
            }

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static char[] Combine(this char[] array, params char[][] others)
        {
            if (others == null)
            {
                throw new ArgumentNullException(nameof(others));
            }

            var result = new char[array.Length + others.Sum(_ => _.Length)];

            Buffer.BlockCopy(array, 0, result, 0, array.Length);

            var offset = array.Length;
            foreach (var other in others)
            {
                Buffer.BlockCopy(other, 0, result, offset, other.Length);
                offset += other.Length;
            }

            return result;
        }

        //--------------------------------------------------------------------------------
        // SubArray
        //--------------------------------------------------------------------------------

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static T[] SubArray<T>(this T[] array, int offset, int length)
        {
            var result = new T[Math.Min(length, array.Length - offset)];

            Array.Copy(array, offset, result, 0, result.Length);

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static byte[] SubArray(this byte[] array, int offset, int length)
        {
            var result = new byte[Math.Min(length, array.Length - offset)];

            Buffer.BlockCopy(array, offset, result, 0, result.Length);

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static short[] SubArray(this short[] array, int offset, int length)
        {
            var result = new short[Math.Min(length, array.Length - offset)];

            Buffer.BlockCopy(array, offset, result, 0, result.Length);

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static int[] SubArray(this int[] array, int offset, int length)
        {
            var result = new int[Math.Min(length, array.Length - offset)];

            Buffer.BlockCopy(array, offset, result, 0, result.Length);

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static long[] SubArray(this long[] array, int offset, int length)
        {
            var result = new long[Math.Min(length, array.Length - offset)];

            Buffer.BlockCopy(array, offset, result, 0, result.Length);

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static float[] SubArray(this float[] array, int offset, int length)
        {
            var result = new float[Math.Min(length, array.Length - offset)];

            Buffer.BlockCopy(array, offset, result, 0, result.Length);

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static double[] SubArray(this double[] array, int offset, int length)
        {
            var result = new double[Math.Min(length, array.Length - offset)];

            Buffer.BlockCopy(array, offset, result, 0, result.Length);

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static bool[] SubArray(this bool[] array, int offset, int length)
        {
            var result = new bool[Math.Min(length, array.Length - offset)];

            Buffer.BlockCopy(array, offset, result, 0, result.Length);

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:パブリック メソッドの引数の検証", Justification = "Extensions")]
        public static char[] SubArray(this char[] array, int offset, int length)
        {
            var result = new char[Math.Min(length, array.Length - offset)];

            Buffer.BlockCopy(array, offset, result, 0, result.Length);

            return result;
        }
    }
}
