namespace Smart.IO.ByteMapper.Helpers
{
    using System;
    using System.Runtime.CompilerServices;

    public static class DecimalHelper
    {
        private const int SignMask = unchecked((int)0x80000000);

        private const int ScaleMask = 0x00FF0000;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal FromBits(int lo, int mid, int hi, int flag)
        {
            return new Decimal(lo, mid, hi, (flag & SignMask) != 0, (byte)((flag & ScaleMask) >> 16));
        }
    }
}