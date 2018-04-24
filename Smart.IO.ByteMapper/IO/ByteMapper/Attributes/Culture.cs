namespace Smart.IO.ByteMapper.Attributes
{
    using System.Collections.Generic;
    using System.Globalization;

    public enum Culture
    {
        Current,
        Invaliant
    }

    public static class CultureExtensions
    {
        private static readonly Dictionary<Culture, CultureInfo> Cultures = new Dictionary<Culture, CultureInfo>
        {
            { Culture.Current, CultureInfo.CurrentCulture },
            { Culture.Invaliant, CultureInfo.InvariantCulture }
        };

        public static CultureInfo ToCultureInfo(this Culture culture)
        {
            return Cultures[culture];
        }
    }
}
