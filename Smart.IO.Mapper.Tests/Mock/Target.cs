namespace Smart.IO.Mapper.Mock
{
    public enum IntEnum
    {
        Zero,
        One,
        Two
    }

    public enum LongEnum : long
    {
        Zero,
        One,
        Two
    }

    public enum ShortEnum : short
    {
        Zero,
        One,
        Two
    }

    public class Target
    {
        // int

        public int IntProperty { get; set; }

        public int? NullableIntProperty { get; set; }

        public IntEnum IntEnumProperty { get; set; }

        public IntEnum? NullableIntEnumProperty { get; set; }

        // long

        public long LongProperty { get; set; }

        public long? NullableLongProperty { get; set; }

        public LongEnum LongEnumProperty { get; set; }

        public LongEnum? NullableLongEnumProperty { get; set; }

        // short

        public short ShortProperty { get; set; }

        public short? NullableShortProperty { get; set; }

        public ShortEnum ShortEnumProperty { get; set; }

        public ShortEnum? NullableShortEnumProperty { get; set; }
    }
}
