namespace Smart.IO.ByteMapper.Builders;

using System.Text;

using Smart.IO.ByteMapper.Converters;

public sealed class GuidTextConverterBuilder : AbstractMapConverterBuilder<GuidTextConverterBuilder>
{
    public int Length { get; set; }

    public string Format { get; set; }

    public Encoding Encoding { get; set; }

    public byte? Filler { get; set; }

    static GuidTextConverterBuilder()
    {
        AddEntry(typeof(Guid), static (b, _) => b.Length, static (b, _, c) => b.CreateGuidTextConverter(c));
        AddEntry(typeof(Guid?), static (b, _) => b.Length, static (b, _, c) => b.CreateNullableGuidTextConverter(c));
    }

    private IMapConverter CreateGuidTextConverter(IBuilderContext context)
    {
        var length = Length;
        var format = Format;
        var encoding = Encoding ?? context.GetParameter<Encoding>(Parameter.Encoding);
        var filler = Filler ?? context.GetParameter<byte>(Parameter.Filler);

        if (TryGetFastFormat(format, out var formatChar, out var width) &&
            width <= length &&
            encoding.GetByteCount(format) == format.Length)
        {
            return new GuidTextFastConverter(length, formatChar, width, filler);
        }

        return new GuidTextConverter(length, format, encoding, filler);
    }

    private IMapConverter CreateNullableGuidTextConverter(IBuilderContext context)
    {
        var length = Length;
        var format = Format;
        var encoding = Encoding ?? context.GetParameter<Encoding>(Parameter.Encoding);
        var filler = Filler ?? context.GetParameter<byte>(Parameter.Filler);

        if (TryGetFastFormat(format, out var formatChar, out var width) &&
            width <= length &&
            encoding.GetByteCount(format) == format.Length)
        {
            return new NullableGuidTextFastConverter(length, formatChar, width, filler);
        }

        return new NullableGuidTextConverter(length, format, encoding, filler);
    }

    private static bool TryGetFastFormat(string format, out char formatChar, out int width)
    {
        formatChar = '\0';
        width = 0;
        if (string.IsNullOrEmpty(format) || format.Length != 1)
        {
            return false;
        }

        formatChar = char.ToUpperInvariant(format[0]);
        width = formatChar switch
        {
            'N' => 32,
            'D' => 36,
            'B' or 'P' => 38,
            _ => 0
        };
        return width > 0;
    }
}
