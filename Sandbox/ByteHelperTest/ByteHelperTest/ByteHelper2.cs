namespace ByteHelperTest
{
    using System;
    using System.Runtime.CompilerServices;

    public static class ByteHelper2
    {
        private const byte Minus = (byte)'-';
        private const byte Num0 = (byte)'0';
        private const byte Dot = (byte)'.';
        private const byte Comma = (byte)',';
        //private const int DotDiff = Dot - Num0;
        //private const int CommaDiff = Comma - Num0;

        private const int Int32MinValueDiv10 = Int32.MinValue / 10;
        private const byte Int32MinValueMod10 = Num0 + -(Int32.MinValue % 10);
        private const long Int64MinValueDiv10 = Int64.MinValue / 10;
        private const byte Int64MinValueMod10 = (byte)(Num0 + -(Int64.MinValue % 10));

        private const int NegativeBitFlag = unchecked((int)0x80000000);

        //--------------------------------------------------------------------------------
        // Integer
        //--------------------------------------------------------------------------------

        // Int32

        public static unsafe void FormatInt32WithTable(byte[] bytes, int index, int length, int value, Padding padding, bool zerofill, byte filler)
        {
            fixed (byte* pBytes = &bytes[index])
            {
                var i = 0;

                if ((value == Int32.MinValue) && (i < length))
                {
                    *(pBytes + i++) = Int32MinValueMod10;
                    value = Int32MinValueDiv10;
                }

                var negative = value < 0;
                if (negative)
                {
                    value = -value;
                }

                var work = stackalloc byte[12];
                var writed = DigitTable.GetIntBuffer12(work, value);

                var copy = writed > length - i ? length - i : writed;
                Buffer.MemoryCopy(work, pBytes + i, copy, copy);
                i += copy;

                if (zerofill)
                {
                    var max = negative ? length - 1 : length;
                    while (i < max)
                    {
                        *(pBytes + i++) = Num0;
                    }

                    if (negative && (i < length))
                    {
                        *(pBytes + i) = Minus;
                    }

                    ReverseBytes(pBytes, length);
                }
                else if (padding == Padding.Left)
                {
                    if (negative && (i < length))
                    {
                        *(pBytes + i++) = Minus;
                    }

                    while (i < length)
                    {
                        *(pBytes + i++) = filler;
                    }

                    ReverseBytes(pBytes, length);
                }
                else
                {
                    if (negative && (i < length))
                    {
                        *(pBytes + i++) = Minus;
                    }

                    ReverseBytes(pBytes, i);

                    while (i < length)
                    {
                        *(pBytes + i++) = filler;
                    }
                }
            }
        }

        public static unsafe void FormatInt32(byte[] bytes, int index, int length, int value, Padding padding, bool zerofill, byte filler)
        {
            fixed (byte* pBytes = &bytes[index])
            {
                if ((padding == Padding.Left) || zerofill)
                {
                    var i = length - 1;

                    if ((value == Int32.MinValue) && (i >= 0))
                    {
                        *(pBytes + i--) = Int32MinValueMod10;
                        value = Int32MinValueDiv10;
                    }

                    var negative = value < 0;
                    if (negative)
                    {
                        value = -value;
                    }

                    while (i >= 0)
                    {
                        *(pBytes + i--) = (byte)(Num0 + (value % 10));

                        value /= 10;
                        if (value == 0)
                        {
                            break;
                        }
                    }

                    if (zerofill)
                    {
                        while (i >= (negative ? 1 : 0))
                        {
                            *(pBytes + i--) = Num0;
                        }

                        if (negative && (i >= 0))
                        {
                            *pBytes = Minus;
                        }
                    }
                    else
                    {
                        if (negative && (i >= 0))
                        {
                            *(pBytes + i--) = Minus;
                        }

                        while (i >= 0)
                        {
                            *(pBytes + i--) = filler;
                        }
                    }
                }
                else
                {
                    var i = 0;

                    if ((value == Int32.MinValue) && (i < length))
                    {
                        *(pBytes + i++) = Int32MinValueMod10;
                        value = Int32MinValueDiv10;
                    }

                    var negative = value < 0;
                    if (negative)
                    {
                        value = -value;
                    }

                    while (i < length)
                    {
                        *(pBytes + i++) = (byte)(Num0 + (value % 10));

                        value /= 10;
                        if (value == 0)
                        {
                            break;
                        }
                    }

                    if (negative && (i < length))
                    {
                        *(pBytes + i++) = Minus;
                    }

                    ReverseBytes(pBytes, i);

                    while (i < length)
                    {
                        *(pBytes + i++) = filler;
                    }
                }
            }
        }

        // Int 64

        public static unsafe void FormatInt64(byte[] bytes, int index, int length, long value, Padding padding, bool zerofill, byte filler)
        {
            fixed (byte* pBytes = &bytes[index])
            {
                if ((padding == Padding.Left) || zerofill)
                {
                    var i = length - 1;

                    if ((value == Int64.MinValue) && (i >= 0))
                    {
                        *(pBytes + i--) = Int64MinValueMod10;
                        value = Int64MinValueDiv10;
                    }

                    var negative = value < 0;
                    if (negative)
                    {
                        value = -value;
                    }

                    while (i >= 0)
                    {
                        *(pBytes + i--) = (byte)(Num0 + (value % 10));

                        value /= 10;
                        if (value == 0)
                        {
                            break;
                        }
                    }

                    if (zerofill)
                    {
                        while (i >= (negative ? 1 : 0))
                        {
                            *(pBytes + i--) = Num0;
                        }

                        if (negative && (i >= 0))
                        {
                            *pBytes = Minus;
                        }
                    }
                    else
                    {
                        if (negative && (i >= 0))
                        {
                            *(pBytes + i--) = Minus;
                        }

                        while (i >= 0)
                        {
                            *(pBytes + i--) = filler;
                        }
                    }
                }
                else
                {
                    var i = 0;

                    if ((value == Int64.MinValue) && (i < length))
                    {
                        *(pBytes + i++) = Int64MinValueMod10;
                        value = Int64MinValueDiv10;
                    }

                    var negative = value < 0;
                    if (negative)
                    {
                        value = -value;
                    }

                    while (i < length)
                    {
                        *(pBytes + i++) = (byte)(Num0 + (value % 10));

                        value /= 10;
                        if (value == 0)
                        {
                            break;
                        }
                    }

                    if (negative && (i < length))
                    {
                        *(pBytes + i++) = Minus;
                    }

                    ReverseBytes(pBytes, i);

                    while (i < length)
                    {
                        *(pBytes + i++) = filler;
                    }
                }
            }
        }

        public static unsafe void FormatInt64WithTable(byte[] bytes, int index, int length, long value, Padding padding, bool zerofill, byte filler)
        {
            fixed (byte* pBytes = &bytes[index])
            {
                var i = 0;

                if ((value == Int64.MinValue) && (i < length))
                {
                    *(pBytes + i++) = Int64MinValueMod10;
                    value = Int64MinValueDiv10;
                }

                var negative = value < 0;
                if (negative)
                {
                    value = -value;
                }

                var work = stackalloc byte[20];
                var writed = DigitTable.GetLongBuffer20(work, value);

                var copy = writed > length - i ? length - i : writed;
                Buffer.MemoryCopy(work, pBytes + i, copy, copy);
                i += copy;

                if (zerofill)
                {
                    var max = negative ? length - 1 : length;
                    while (i < max)
                    {
                        *(pBytes + i++) = Num0;
                    }

                    if (negative && (i < length))
                    {
                        *(pBytes + i) = Minus;
                    }

                    ReverseBytes(pBytes, length);
                }
                else if (padding == Padding.Left)
                {
                    if (negative && (i < length))
                    {
                        *(pBytes + i++) = Minus;
                    }

                    while (i < length)
                    {
                        *(pBytes + i++) = filler;
                    }

                    ReverseBytes(pBytes, length);
                }
                else
                {
                    if (negative && (i < length))
                    {
                        *(pBytes + i++) = Minus;
                    }

                    ReverseBytes(pBytes, i);

                    while (i < length)
                    {
                        *(pBytes + i++) = filler;
                    }
                }
            }
        }

        //--------------------------------------------------------------------------------
        // Decimal
        //--------------------------------------------------------------------------------

        public static unsafe void FormatDecimalLimited64(
    byte[] bytes,
    int index,
    int length,
    decimal value,
    byte scale,
    int groupingSize,
    Padding padding,
    bool zerofill,
    byte filler)
        {
            var bits = Decimal.GetBits(value);
            var negative = (bits[3] & NegativeBitFlag) != 0;
            var decimalScale = (bits[3] >> 16) & 0x7F;
            var decimalNum = ((ulong)(bits[1] & 0x00000000FFFFFFFF) << 32) + (ulong)(bits[0] & 0x00000000FFFFFFFF);

            var work = stackalloc byte[30];
            var workSize = 0;
            var workPointer = 0;

            while (decimalNum > Int64.MaxValue)
            {
                work[workSize++] = (byte)(decimalNum % 10);
                decimalNum /= 10;
            }

            var decimalNum2 = (long)decimalNum;
            while (decimalNum2 > 0)
            {
                work[workSize++] = (byte)(decimalNum2 % 10);
                decimalNum2 /= 10;
            }

            // Fix Scale
            if (scale < decimalScale)
            {
                workPointer = decimalScale - scale;
                if (work[workPointer - 1] > 4)
                {
                    var i = workPointer;
                    var carry = true;
                    while (carry && (i < workSize))
                    {
                        if (work[i] == 9)
                        {
                            work[i++] = 0;
                        }
                        else
                        {
                            work[i] += 1;
                            carry = false;
                        }
                    }

                    if (carry)
                    {
                        workSize++;
                        work[i] = 1;
                    }
                }
            }

            fixed (byte* pBytes = &bytes[index])
            {
                if ((padding == Padding.Left) || zerofill)
                {
                    var i = length - 1;

                    if (scale > 0)
                    {
                        var dotPos = length - scale - 1;

                        var completion = scale - decimalScale;
                        while ((completion > 0) && (i >= 0))
                        {
                            *(pBytes + i--) = Num0;
                            completion--;
                        }

                        while ((i > dotPos) && (i >= 0))
                        {
                            if (workPointer < workSize)
                            {
                                *(pBytes + i--) = (byte)(Num0 + work[workPointer++]);
                            }
                            else
                            {
                                *(pBytes + i--) = Num0;
                            }
                        }

                        if ((i == dotPos) && (i >= 0))
                        {
                            *(pBytes + i--) = Dot;
                        }
                    }

                    var groupingCount = 0;

                    if ((workPointer == workSize) && (i >= 0))
                    {
                        *(pBytes + i--) = Num0;
                    }
                    else
                    {
                        while ((workPointer < workSize) && (i >= 0))
                        {
                            if (groupingCount == groupingSize)
                            {
                                *(pBytes + i--) = Comma;
                                groupingCount = 0;

                                if (i < 0)
                                {
                                    break;
                                }
                            }

                            *(pBytes + i--) = (byte)(Num0 + work[workPointer++]);

                            groupingCount++;
                        }
                    }

                    if (zerofill)
                    {
                        var end = negative ? 1 : 0;
                        while (i >= end)
                        {
                            if (groupingCount == groupingSize)
                            {
                                *(pBytes + i--) = Comma;
                                groupingCount = 0;

                                if (i < end)
                                {
                                    break;
                                }
                            }

                            *(pBytes + i--) = Num0;

                            groupingCount++;
                        }

                        if (negative && (i >= 0))
                        {
                            *pBytes = Minus;
                        }
                    }
                    else
                    {
                        if (negative && (i >= 0))
                        {
                            *(pBytes + i--) = Minus;
                        }

                        while (i >= 0)
                        {
                            *(pBytes + i--) = filler;
                        }
                    }
                }
                else
                {
                    var i = 0;

                    if (scale > 0)
                    {
                        var dotPos = scale;

                        var completion = scale - decimalScale;
                        while ((completion > 0) && (i < length))
                        {
                            *(pBytes + i++) = Num0;
                            completion--;
                        }

                        while ((i < dotPos) && (i < length))
                        {
                            if (workPointer < workSize)
                            {
                                *(pBytes + i++) = (byte)(Num0 + work[workPointer++]);
                            }
                            else
                            {
                                *(pBytes + i++) = Num0;
                            }
                        }

                        if ((i == dotPos) && (i < length))
                        {
                            *(pBytes + i++) = Dot;
                        }
                    }

                    var groupingCount = 0;

                    if ((workPointer == workSize) && (i < length))
                    {
                        *(pBytes + i++) = Num0;
                    }
                    else
                    {
                        while ((workPointer < workSize) && (i < length))
                        {
                            if (groupingCount == groupingSize)
                            {
                                *(pBytes + i++) = Comma;
                                groupingCount = 0;

                                if (i >= length)
                                {
                                    break;
                                }
                            }

                            *(pBytes + i++) = (byte)(Num0 + work[workPointer++]);

                            groupingCount++;
                        }
                    }

                    if (negative && (i < length))
                    {
                        *(pBytes + i++) = Minus;
                    }

                    ReverseBytes(pBytes, i);

                    while (i < length)
                    {
                        *(pBytes + i++) = filler;
                    }
                }
            }
        }

        public static unsafe void FormatDecimal(
            byte[] bytes,
            int index,
            int length,
            decimal value,
            byte scale,
            int groupingSize,
            Padding padding,
            bool zerofill,
            byte filler)
        {
            var bits = Decimal.GetBits(value);
            var negative = (bits[3] & NegativeBitFlag) != 0;
            var decimalScale = (bits[3] >> 16) & 0x7F;

            var lo = 0L;
            var hi = 0L;
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 0, (bits[0] >> 0) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 1, (bits[0] >> 8) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 2, (bits[0] >> 16) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 3, (bits[0] >> 24) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 4, (bits[1] >> 0) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 5, (bits[1] >> 8) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 6, (bits[1] >> 16) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 7, (bits[1] >> 24) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 8, (bits[2] >> 0) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 9, (bits[2] >> 8) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 10, (bits[2] >> 16) & 0b11111111);
            DecimalTable.AddBitBlockValue(ref lo, ref hi, 11, (bits[2] >> 24) & 0b11111111);

            var work = stackalloc byte[30];
            var workSize = 0;
            var workPointer = 0;

            while (lo > 0)
            {
                work[workSize++] = (byte)(lo % 10);
                lo /= 10;
            }

            if (hi > 0)
            {
                workSize = 15;
                while (hi > 0)
                {
                    work[workSize++] = (byte)(hi % 10);
                    hi /= 10;
                }
            }

            // Fix Scale
            if (scale < decimalScale)
            {
                workPointer = decimalScale - scale;
                if (work[workPointer - 1] > 4)
                {
                    var i = workPointer;
                    var carry = true;
                    while (carry && (i < workSize))
                    {
                        if (work[i] == 9)
                        {
                            work[i++] = 0;
                        }
                        else
                        {
                            work[i] += 1;
                            carry = false;
                        }
                    }

                    if (carry)
                    {
                        workSize++;
                        work[i] = 1;
                    }
                }
            }

            fixed (byte* pBytes = &bytes[index])
            {
                if ((padding == Padding.Left) || zerofill)
                {
                    var i = length - 1;

                    if (scale > 0)
                    {
                        var dotPos = length - scale - 1;

                        var completion = scale - decimalScale;
                        while ((completion > 0) && (i >= 0))
                        {
                            *(pBytes + i--) = Num0;
                            completion--;
                        }

                        while ((i > dotPos) && (i >= 0))
                        {
                            if (workPointer < workSize)
                            {
                                *(pBytes + i--) = (byte)(Num0 + work[workPointer++]);
                            }
                            else
                            {
                                *(pBytes + i--) = Num0;
                            }
                        }

                        if ((i == dotPos) && (i >= 0))
                        {
                            *(pBytes + i--) = Dot;
                        }
                    }

                    var groupingCount = 0;

                    if ((workPointer == workSize) && (i >= 0))
                    {
                        *(pBytes + i--) = Num0;
                    }
                    else
                    {
                        while ((workPointer < workSize) && (i >= 0))
                        {
                            if (groupingCount == groupingSize)
                            {
                                *(pBytes + i--) = Comma;
                                groupingCount = 0;

                                if (i < 0)
                                {
                                    break;
                                }
                            }

                            *(pBytes + i--) = (byte)(Num0 + work[workPointer++]);

                            groupingCount++;
                        }
                    }

                    if (zerofill)
                    {
                        var end = negative ? 1 : 0;
                        while (i >= end)
                        {
                            if (groupingCount == groupingSize)
                            {
                                *(pBytes + i--) = Comma;
                                groupingCount = 0;

                                if (i < end)
                                {
                                    break;
                                }
                            }

                            *(pBytes + i--) = Num0;

                            groupingCount++;
                        }

                        if (negative && (i >= 0))
                        {
                            *pBytes = Minus;
                        }
                    }
                    else
                    {
                        if (negative && (i >= 0))
                        {
                            *(pBytes + i--) = Minus;
                        }

                        while (i >= 0)
                        {
                            *(pBytes + i--) = filler;
                        }
                    }
                }
                else
                {
                    var i = 0;

                    if (scale > 0)
                    {
                        var dotPos = scale;

                        var completion = scale - decimalScale;
                        while ((completion > 0) && (i < length))
                        {
                            *(pBytes + i++) = Num0;
                            completion--;
                        }

                        while ((i < dotPos) && (i < length))
                        {
                            if (workPointer < workSize)
                            {
                                *(pBytes + i++) = (byte)(Num0 + work[workPointer++]);
                            }
                            else
                            {
                                *(pBytes + i++) = Num0;
                            }
                        }

                        if ((i == dotPos) && (i < length))
                        {
                            *(pBytes + i++) = Dot;
                        }
                    }

                    var groupingCount = 0;

                    if ((workPointer == workSize) && (i < length))
                    {
                        *(pBytes + i++) = Num0;
                    }
                    else
                    {
                        while ((workPointer < workSize) && (i < length))
                        {
                            if (groupingCount == groupingSize)
                            {
                                *(pBytes + i++) = Comma;
                                groupingCount = 0;

                                if (i >= length)
                                {
                                    break;
                                }
                            }

                            *(pBytes + i++) = (byte)(Num0 + work[workPointer++]);

                            groupingCount++;
                        }
                    }

                    if (negative && (i < length))
                    {
                        *(pBytes + i++) = Minus;
                    }

                    ReverseBytes(pBytes, i);

                    while (i < length)
                    {
                        *(pBytes + i++) = filler;
                    }
                }
            }
        }

        //public static unsafe void FormatDecimalWithTable(
        //    byte[] bytes,
        //    int offset,
        //    int length,
        //    decimal value)
        //{
        //    var bits = Decimal.GetBits(value);

        //    var lo = 0L;
        //    var hi = 0L;
        //    DecimalTable.AddBitBlockValue(ref lo, ref hi, 0, (bits[0] >> 0) & 0b11111111);
        //    DecimalTable.AddBitBlockValue(ref lo, ref hi, 1, (bits[0] >> 8) & 0b11111111);
        //    DecimalTable.AddBitBlockValue(ref lo, ref hi, 2, (bits[0] >> 16) & 0b11111111);
        //    DecimalTable.AddBitBlockValue(ref lo, ref hi, 3, (bits[0] >> 24) & 0b11111111);
        //    DecimalTable.AddBitBlockValue(ref lo, ref hi, 4, (bits[1] >> 0) & 0b11111111);
        //    DecimalTable.AddBitBlockValue(ref lo, ref hi, 5, (bits[1] >> 8) & 0b11111111);
        //    DecimalTable.AddBitBlockValue(ref lo, ref hi, 6, (bits[1] >> 16) & 0b11111111);
        //    DecimalTable.AddBitBlockValue(ref lo, ref hi, 7, (bits[1] >> 24) & 0b11111111);
        //    DecimalTable.AddBitBlockValue(ref lo, ref hi, 8, (bits[2] >> 0) & 0b11111111);
        //    DecimalTable.AddBitBlockValue(ref lo, ref hi, 9, (bits[2] >> 8) & 0b11111111);
        //    DecimalTable.AddBitBlockValue(ref lo, ref hi, 10, (bits[2] >> 16) & 0b11111111);
        //    DecimalTable.AddBitBlockValue(ref lo, ref hi, 11, (bits[2] >> 24) & 0b11111111);

        //    var work = stackalloc byte[32];
        //    var writed = DigitTable.GetLongBuffer16(work, lo);
        //    writed += DigitTable.GetLongBuffer16(work + 16, hi);

        //    fixed (byte* pBytes = &bytes[offset])
        //    {
        //        var copy = writed > length ? length : writed;
        //        Buffer.MemoryCopy(work, pBytes, copy, copy);

        //        for (var i = writed; i < length; i++)
        //        {
        //            pBytes[offset + i] = 0x30;
        //        }

        //        ReverseBytes(pBytes, length);
        //    }
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void ReverseBytes(byte* ptr, int length)
        {
            var start = ptr;
            var end = ptr + length - 1;
            while (start < end)
            {
                var tmp = *start;
                *start = *end;
                *end = tmp;
                start++;
                end--;
            }
        }
    }
}
