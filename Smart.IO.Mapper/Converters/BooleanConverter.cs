namespace Smart.IO.Mapper.Converters
{
    internal sealed class BooleanConverter : IMapConverter
    {
        private readonly byte trueValue;

        private readonly byte falseValue;

        public BooleanConverter(byte trueValue, byte falseValue)
        {
            this.trueValue = trueValue;
            this.falseValue = falseValue;
        }

        public object Read(byte[] buffer, int index)
        {
            return buffer[index] == trueValue;
        }

        public void Write(byte[] buffer, int index, object value)
        {
            buffer[index] = (bool)value ? trueValue : falseValue;
        }
    }

    internal sealed class NullableBooleanConverter : IMapConverter
    {
        private readonly byte trueValue;

        private readonly byte falseValue;

        private readonly byte nullValue;

        public NullableBooleanConverter(byte trueValue, byte falseValue, byte nullValue)
        {
            this.trueValue = trueValue;
            this.falseValue = falseValue;
            this.nullValue = nullValue;
        }

        public object Read(byte[] buffer, int index)
        {
            var b = buffer[index];
            return b == trueValue ? true : b == nullValue ? (bool?)null : false;
        }

        public void Write(byte[] buffer, int index, object value)
        {
            var b = (bool?)value;
            buffer[index] = b.HasValue ? (b.Value ? trueValue : falseValue) : nullValue;
        }
    }
}
