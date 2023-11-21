namespace Smart.IO.ByteMapper.Builders;

using Smart.IO.ByteMapper.Converters;

public sealed class IntegerConverterBuilder : AbstractMapConverterBuilder<IntegerConverterBuilder>
{
    public int Length { get; set; }

    public Padding? Padding { get; set; }

    public bool? ZeroFill { get; set; }

    public byte? Filler { get; set; }

    static IntegerConverterBuilder()
    {
        AddEntry(typeof(int), static (b, _) => b.Length, static (b, t, c) => b.CreateInt32Converter(t, c));
        AddEntry(typeof(int?), static (b, _) => b.Length, static (b, t, c) => b.CreateInt32Converter(t, c));
        AddEntry(typeof(long), static (b, _) => b.Length, static (b, t, c) => b.CreateInt64Converter(t, c));
        AddEntry(typeof(long?), static (b, _) => b.Length, static (b, t, c) => b.CreateInt64Converter(t, c));
        AddEntry(typeof(short), static (b, _) => b.Length, static (b, t, c) => b.CreateInt16Converter(t, c));
        AddEntry(typeof(short?), static (b, _) => b.Length, static (b, t, c) => b.CreateInt16Converter(t, c));
    }

    private Int32Converter CreateInt32Converter(Type type, IBuilderContext context)
    {
        return new Int32Converter(
            Length,
            Padding ?? context.GetParameter<Padding>(OptionsParameter.NumberPadding),
            ZeroFill ?? context.GetParameter<bool>(OptionsParameter.ZeroFill),
            Filler ?? context.GetParameter<byte>(OptionsParameter.NumberFiller),
            type);
    }

    private Int64Converter CreateInt64Converter(Type type, IBuilderContext context)
    {
        return new Int64Converter(
            Length,
            Padding ?? context.GetParameter<Padding>(OptionsParameter.NumberPadding),
            ZeroFill ?? context.GetParameter<bool>(OptionsParameter.ZeroFill),
            Filler ?? context.GetParameter<byte>(OptionsParameter.NumberFiller),
            type);
    }

    private Int16Converter CreateInt16Converter(Type type, IBuilderContext context)
    {
        return new Int16Converter(
            Length,
            Padding ?? context.GetParameter<Padding>(OptionsParameter.NumberPadding),
            ZeroFill ?? context.GetParameter<bool>(OptionsParameter.ZeroFill),
            Filler ?? context.GetParameter<byte>(OptionsParameter.NumberFiller),
            type);
    }
}
