namespace ByteHelperTest
{
    using System.Runtime.CompilerServices;

    internal partial class DecimalTable
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddBitBlockValue(ref long lo, ref long hi, int block, int bits)
        {
            var baseIndex = (block << 9) + (bits << 1);

            int carry;
            var value = lo + Table[baseIndex];
            if (value >= 1000000000000000L)
            {
                lo = value - 1000000000000000L;
                carry = 1;
            }
            else
            {
                lo = value;
                carry = 0;
            }

            value = hi + Table[baseIndex + 1] + carry;
            if (value >= 1000000000000000L)
            {
                hi = value - 1000000000000000L;
            }
            else
            {
                hi = value;
            }
        }
    }
}
