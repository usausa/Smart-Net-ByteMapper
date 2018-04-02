namespace Smart.IO.Mapper.Attributes
{
    using System.Globalization;

    public class MapNumberTextAttributeTest
    {
        private const byte CustomFiller = 0x30;

        //--------------------------------------------------------------------------------
        // Attribute
        //--------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------
        // Fix
        //--------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------
        // Helper
        //--------------------------------------------------------------------------------

        [Map(36)]
        internal class NumberTextObject
        {
            [MapNumberText(0, 4)]
            public int IntValue { get; set; }

            [MapNumberText(4, 4)]
            public int? NullableIntValue { get; set; }

            // TODO Provider
            [MapNumberText(8, 4, Encoding = "ASCII", Trim = true, Padding = Padding.Right, Filler = CustomFiller, Style = NumberStyles.Any)]
            public int CustomIntValue { get; set; }

            [MapNumberText(12, 6)]
            public long LongValue { get; set; }

            [MapNumberText(18, 6)]
            public long? NullableLongValue { get; set; }

            // TODO Provider
            [MapNumberText(24, 6, Encoding = "ASCII", Trim = true, Padding = Padding.Right, Filler = CustomFiller, Style = NumberStyles.Any)]
            public long CustomLongValue { get; set; }

            [MapNumberText(30, 2)]
            public short ShortValue { get; set; }

            [MapNumberText(32, 2)]
            public short? NullableShortValue { get; set; }

            // TODO Provider
            [MapNumberText(34, 2, Encoding = "ASCII", Trim = true, Padding = Padding.Right, Filler = CustomFiller, Style = NumberStyles.Any)]
            public short CustomShortValue { get; set; }
        }
    }
}
