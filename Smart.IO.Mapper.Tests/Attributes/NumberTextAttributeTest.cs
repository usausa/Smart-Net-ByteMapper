namespace Smart.IO.Mapper.Attributes
{
    using System.Globalization;

    public class NumberTextAttributeTest
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

        internal class NumberTextObject
        {
            [NumberText(0, 4)]
            public int IntValue { get; set; }

            [NumberText(4, 4)]
            public int? NullableIntValue { get; set; }

            // TODO Provider
            [NumberText(8, 4, Encoding = "ASCII", Trim = true, Padding = Padding.Right, Filler = CustomFiller, Style = NumberStyles.Any)]
            public int CustomIntValue { get; set; }

            [NumberText(12, 6)]
            public long LongValue { get; set; }

            [NumberText(18, 6)]
            public long? NullableLongValue { get; set; }

            // TODO Provider
            [NumberText(24, 6, Encoding = "ASCII", Trim = true, Padding = Padding.Right, Filler = CustomFiller, Style = NumberStyles.Any)]
            public long CustomLongValue { get; set; }

            [NumberText(30, 2)]
            public short ShortValue { get; set; }

            [NumberText(32, 2)]
            public short? NullableShortValue { get; set; }

            // TODO Provider
            [NumberText(34, 2, Encoding = "ASCII", Trim = true, Padding = Padding.Right, Filler = CustomFiller, Style = NumberStyles.Any)]
            public short CustomShortValue { get; set; }
        }
    }
}
