namespace ByteHelper;

using System.Runtime.CompilerServices;

internal static class OperationHelper
{
    private const long InvDivisor = 0x1999999A;

    private const int ModEntryMax = 100;

    private static readonly DivMod10Entry[] DivMod10Entries = new DivMod10Entry[ModEntryMax];

    static OperationHelper()
    {
        for (var i = 0; i < ModEntryMax; i++)
        {
            DivMod10Entries[i] = new DivMod10Entry(Div10Signed(i), i % 10);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Div10Signed(int dividend)
    {
        // signed only
        return (int)((InvDivisor * dividend) >> 32);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DivMod10Signed(int value, out int div, out int mod)
    {
        if (value < ModEntryMax)
        {
            var entry = DivMod10Entries[value];
            div = entry.Div;
            mod = entry.Mod;
        }
        else
        {
            div = Div10Signed(value);
            mod = value % 10;
        }
    }

    private class DivMod10Entry
    {
        public int Div { get; }

        public int Mod { get; }

        public DivMod10Entry(int div, int mod)
        {
            Div = div;
            Mod = mod;
        }
    }
}
