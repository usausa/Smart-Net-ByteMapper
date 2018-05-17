namespace Smart.IO.ByteMapper.Helpers
{
    using System;
    using System.Runtime.CompilerServices;

    public static class DateTimeHelper
    {
        private const long MaxOffset = 60 * 14;
        private const long MinOffset = -MaxOffset;

        private static readonly long MaxTicks = DateTime.MaxValue.Ticks;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidTicks(long value)
        {
            return value >= 0 && value <= MaxTicks;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidOffset(short value)
        {
            return value >= MinOffset && value <= MaxOffset;
        }
    }
}
