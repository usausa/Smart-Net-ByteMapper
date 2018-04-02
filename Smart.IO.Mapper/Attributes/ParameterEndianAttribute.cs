namespace Smart.IO.Mapper.Attributes
{
    public class EndianAttribute : OptionParameterAttribute
    {
        public EndianAttribute(Endian endian)
            : base(Parameter.Endian, endian)
        {
        }
    }

    public class BigEndianAttribute : EndianAttribute
    {
        public BigEndianAttribute()
            : base(Endian.Big)
        {
        }
    }

    public class LittleEndianAttribute : EndianAttribute
    {
        public LittleEndianAttribute()
            : base(Endian.Little)
        {
        }
    }
}
