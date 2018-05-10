namespace ByteHelperTest
{
    using System;

    public static class ByteHelper2
    {
        private const byte Zero = (byte)'0';

        public static unsafe void FormatDecimal0(
            byte[] bytes,
            int offset,
            int length,
            decimal value)
        {
            var bits = Decimal.GetBits(value);

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

            var work = stackalloc byte[Math.Max(length, 32)];
            IntegerTable.GetLongBuffer16(work, lo);
            IntegerTable.GetLongBuffer16(work + 16, hi);

            // TODO tune, reverse
            for (var i = 0; i < length; i++)
            {
                bytes[offset + i] = (byte)(work[i] + Zero);
            }
        }
    }
}
