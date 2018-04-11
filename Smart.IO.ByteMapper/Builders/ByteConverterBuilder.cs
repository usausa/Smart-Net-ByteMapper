namespace Smart.IO.ByteMapper.Builders
{
    using Smart.IO.ByteMapper.Converters;

    public sealed class ByteConverterBuilder : AbstractMapConverterBuilder<ByteConverterBuilder>
    {
        public static ByteConverterBuilder Default { get; } = new ByteConverterBuilder();

        static ByteConverterBuilder()
        {
            AddEntry(typeof(byte), 1, (b, t, c) => ByteConverter.Default);
        }
    }
}
