namespace Smart.IO.ByteMapper.Attributes;

using System.Globalization;

public enum Culture
{
    Current,
    Invariant
}

public static class CultureExtensions
{
    private static readonly Dictionary<Culture, CultureInfo> Cultures = new()
    {
        { Culture.Current, CultureInfo.CurrentCulture },
        { Culture.Invariant, CultureInfo.InvariantCulture }
    };

    public static CultureInfo ToCultureInfo(this Culture culture)
    {
        return Cultures[culture];
    }
}
