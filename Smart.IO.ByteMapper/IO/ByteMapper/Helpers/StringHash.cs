namespace Smart.IO.ByteMapper.Helpers;

using System.Runtime.CompilerServices;

internal static class StringHash
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Calc(string value)
    {
        unchecked
        {
            var hash = 2166136261u;
            foreach (var c in value)
            {
                hash = (c ^ hash) * 16777619;
            }
            return (int)hash;
        }
    }
}
