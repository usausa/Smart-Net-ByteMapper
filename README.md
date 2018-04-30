# Smart.IO.ByteMapper .NET - byte array mapper library for .NET

## What is this ?

* byte array <-> object mapper.
* Configuration by attribute and expressions support.
* Converter can be specified for each property.
* Can change default setting for factory and type.
* Supports different profiles for the same type.
* ASP.NET Core integration.
* Fast custom converter options exist.

### Usage example

```csharp
// Data class
public class ComplexData
{
    public string StringValue1 { get; set; }
    public string StringValue2 { get; set; }
    public string StringValue3 { get; set; }
    public int IntValue1 { get; set; }
    public int IntValue2 { get; set; }
    public int? IntValue3 { get; set; }
    public int? IntValue4 { get; set; }
    public decimal DecimalValue1 { get; set; }
    public decimal? DecimalValue2 { get; set; }
    public bool BoolValue1 { get; set; }
    public bool? BoolValue2 { get; set; }
    public DateTime DateTimeValue1 { get; set; }
    public DateTime? DateTimeValue2 { get; set; }
}
```

```csharp
// Configration by expression
var mapperFactory = new MapperFactoryConfig()
    .UseOptionsDefault()
    .DefaultEncoding(Encoding.GetEncoding(932))
    .CreateMapByExpression<ComplexData>(144, config => config
        .ForMember(x => x.StringValue1, m => m.Text(20))
        .ForMember(x => x.StringValue2, m => m.Text(20))
        .ForMember(x => x.StringValue3, m => m.Text(20))
        .ForMember(x => x.IntValue1, m => m.Integer(8))
        .ForMember(x => x.IntValue2, m => m.Integer(8))
        .ForMember(x => x.IntValue3, m => m.Integer(8))
        .ForMember(x => x.IntValue4, m => m.Integer(8))
        .ForMember(x => x.DecimalValue1, m => m.Decimal(10, 2))
        .ForMember(x => x.DecimalValue2, m => m.Decimal(10, 2))
        .ForMember(x => x.BoolValue1, m => m.Boolean())
        .ForMember(x => x.BoolValue2, m => m.Boolean())
        .ForMember(x => x.DateTimeValue1, m => m.DateTime("yyyyMMddHHmmss"))
        .ForMember(x => x.DateTimeValue2, m => m.DateTime("yyyyMMddHHmmss")))
    .ToMapperFactory();

// Create type mapper
var mapper = mapperFactory.Create<ComplexData>();
```

```csharp
// Usage
var buffer = new byte[mapper.Size];
var data = new ComplexData
{
    StringValue1 = "XXXXXXXXXXXXXXXXXXXX",
    StringValue2 = "あああああ",
    StringValue3 = string.Empty,
    IntValue1 = 1,
    IntValue2 = 0,
    IntValue3 = 1,
    IntValue4 = null,
    BoolValue1 = true,
    BoolValue2 = null,
    DecimalValue1 = 1.23m,
    DecimalValue2 = null,
    DateTimeValue1 = new DateTime(2000, 12, 31, 23, 59, 59, 999),
    DateTimeValue2 = null
};

// object to byte array
mapper.ToByte(buffer, 0, data);
// buffer
// [XXXXXXXXXXXXXXXXXXXXあああああ                                     1       0       1              1.23          1 20001231235959              \r\n]

// byte array to object
mapper.FromByte(buffer, 0, data);

```

Performance in this case.

* 1,500,000 op/s byte array to object
* 1,050,000 op/s object to byte array

## NuGet

| Id                                 | Note                       |
|------------------------------------|----------------------------|
| Usa.Smart.IO.ByteMapper            | Core libyrary              |
| Usa.Smart.IO.ByteMapper.Options    | Optional converters        |
| Usa.Smart.IO.ByteMapper.AspNetCore | ASP.NET Core integration   |

## Benchmark

Converter benchmark in .NET Core 2.0.

|                       Method |       Mean |       Error |     StdDev |  Gen 0 | Allocated |
|----------------------------- |-----------:|------------:|-----------:|-------:|----------:|
|                ReadIntBinary |   6.308 ns |   1.5587 ns |  0.0881 ns | 0.0057 |      24 B |
|                  ReadBoolean |   4.864 ns |   0.1343 ns |  0.0076 ns | 0.0057 |      24 B |
|                  ReadBytes10 |  13.378 ns |   3.1810 ns |  0.1797 ns | 0.0095 |      40 B |
|                  ReadBytes20 |  13.777 ns |   1.6255 ns |  0.0918 ns | 0.0114 |      48 B |
|       ReadSjisText20Single20 | 114.122 ns |   3.9026 ns |  0.2205 ns | 0.0170 |      72 B |
|          ReadSjisText20Wide5 |  81.905 ns |   5.9850 ns |  0.3382 ns | 0.0094 |      40 B |
|          ReadSjisText20Empty |  36.883 ns |  17.1153 ns |  0.9670 ns |      - |       0 B |
|     ReadNumberTextShort4Zero |  94.412 ns |   5.1714 ns |  0.2922 ns | 0.0132 |      56 B |
|      ReadNumberTextShort4Max | 110.853 ns |  13.0244 ns |  0.7359 ns | 0.0151 |      64 B |
|       ReadNumberTextInt8Zero |  93.061 ns |   4.1243 ns |  0.2330 ns | 0.0132 |      56 B |
|        ReadNumberTextInt8Max | 132.318 ns |   6.7653 ns |  0.3823 ns | 0.0172 |      72 B |
|     ReadNumberTextLong18Zero |  98.981 ns |   6.6714 ns |  0.3769 ns | 0.0132 |      56 B |
|      ReadNumberTextLong18Max | 198.732 ns |  27.3234 ns |  1.5438 ns | 0.0207 |      88 B |
|   ReadNumberTextDecimal8Zero | 111.449 ns |   7.9690 ns |  0.4503 ns | 0.0151 |      64 B |
|    ReadNumberTextDecimal8Max | 218.369 ns |  12.7117 ns |  0.7182 ns | 0.0188 |      80 B |
|  ReadNumberTextDecimal18Zero | 124.317 ns |   8.0663 ns |  0.4558 ns | 0.0169 |      72 B |
|   ReadNumberTextDecimal18Max | 409.370 ns |  35.1513 ns |  1.9861 ns | 0.0224 |      96 B |
|            ReadDateTimeText8 | 257.702 ns |   2.7524 ns |  0.1555 ns | 0.0172 |      72 B |
|           ReadDateTimeText14 | 384.261 ns | 256.6164 ns | 14.4993 ns | 0.0186 |      80 B |
|           ReadDateTimeText17 | 458.880 ns |   9.8543 ns |  0.5568 ns | 0.0205 |      88 B |
|               ReadText13Code |  29.843 ns |   2.6018 ns |  0.1470 ns | 0.0133 |      56 B |
|              ReadAscii13Code |  17.813 ns |   0.9506 ns |  0.0537 ns | 0.0133 |      56 B |
|        ReadIntegerShort4Zero |  11.834 ns |   0.7269 ns |  0.0411 ns | 0.0057 |      24 B |
|         ReadIntegerShort4Max |  13.894 ns |   1.2671 ns |  0.0716 ns | 0.0057 |      24 B |
|             ReadInteger8Zero |  12.544 ns |   0.7409 ns |  0.0419 ns | 0.0057 |      24 B |
|              ReadInteger8Max |  18.477 ns |   0.5558 ns |  0.0314 ns | 0.0057 |      24 B |
|               ReadLong18Zero |  17.548 ns |   0.3739 ns |  0.0211 ns | 0.0057 |      24 B |
|                ReadLong18Max |  28.339 ns |   2.2402 ns |  0.1266 ns | 0.0057 |      24 B |
|             ReadDecimal8Zero |  21.939 ns |   3.0054 ns |  0.1698 ns | 0.0076 |      32 B |
|              ReadDecimal8Max |  24.707 ns |   7.3059 ns |  0.4128 ns | 0.0076 |      32 B |
|            ReadDecimal18Zero |  26.653 ns |   0.9961 ns |  0.0563 ns | 0.0076 |      32 B |
|             ReadDecimal18Max |  34.520 ns |   4.2505 ns |  0.2402 ns | 0.0076 |      32 B |
|                ReadDateTime8 |  40.689 ns |  10.8581 ns |  0.6135 ns | 0.0057 |      24 B |
|               ReadDateTime14 |  53.432 ns |   1.9964 ns |  0.1128 ns | 0.0057 |      24 B |
|               ReadDateTime17 |  66.813 ns |   2.0077 ns |  0.1134 ns | 0.0056 |      24 B |

|                       Method |       Mean |       Error |     StdDev |  Gen 0 | Allocated |
|----------------------------- |-----------:|------------:|-----------:|-------:|----------:|
|               WriteIntBinary |   6.319 ns |   0.6598 ns |  0.0373 ns | 0.0057 |      24 B |
|                 WriteBoolean |   5.558 ns |   0.5231 ns |  0.0296 ns | 0.0057 |      24 B |
|                 WriteBytes10 |  12.910 ns |   0.3403 ns |  0.0192 ns |      - |       0 B |
|                 WriteBytes20 |  12.866 ns |   0.7765 ns |  0.0439 ns |      - |       0 B |
|      WriteSjisText20Single20 | 156.843 ns |   3.6737 ns |  0.2076 ns | 0.0112 |      48 B |
|         WriteSjisText20Wide5 |  86.179 ns |  39.6817 ns |  2.2421 ns | 0.0094 |      40 B |
|         WriteSjisText20Empty |  74.409 ns |   9.0488 ns |  0.5113 ns | 0.0132 |      56 B |
|    WriteNumberTextShort4Zero |  87.239 ns |   7.2761 ns |  0.4111 ns | 0.0209 |      88 B |
|     WriteNumberTextShort4Max |  94.024 ns |   4.3919 ns |  0.2482 ns | 0.0228 |      96 B |
|      WriteNumberTextInt8Zero |  88.200 ns |   9.3780 ns |  0.5299 ns | 0.0209 |      88 B |
|       WriteNumberTextInt8Max | 104.536 ns |  12.8731 ns |  0.7274 ns | 0.0247 |     104 B |
|    WriteNumberTextLong18Zero |  96.157 ns |   7.2108 ns |  0.4074 ns | 0.0209 |      88 B |
|     WriteNumberTextLong18Max | 151.065 ns |   7.6594 ns |  0.4328 ns | 0.0322 |     136 B |
|  WriteNumberTextDecimal8Zero | 108.482 ns |   3.7677 ns |  0.2129 ns | 0.0228 |      96 B |
|   WriteNumberTextDecimal8Max | 147.024 ns |   9.8697 ns |  0.5577 ns | 0.0284 |     120 B |
| WriteNumberTextDecimal18Zero | 114.763 ns |  16.8624 ns |  0.9528 ns | 0.0228 |      96 B |
|  WriteNumberTextDecimal18Max | 209.545 ns |  22.1247 ns |  1.2501 ns | 0.0343 |     144 B |
|           WriteDateTimeText8 | 338.550 ns | 173.4578 ns |  9.8007 ns | 0.0491 |     208 B |
|          WriteDateTimeText14 | 414.802 ns |  28.1930 ns |  1.5930 ns | 0.0529 |     224 B |
|          WriteDateTimeText17 | 583.249 ns |  33.8503 ns |  1.9126 ns | 0.0639 |     272 B |
|              WriteText13Code |  46.324 ns |   1.3687 ns |  0.0773 ns | 0.0095 |      40 B |
|             WriteAscii13Code |  23.670 ns |   0.5271 ns |  0.0298 ns | 0.0095 |      40 B |
|       WriteIntegerShort4Zero |  14.454 ns |   2.0485 ns |  0.1157 ns | 0.0057 |      24 B |
|        WriteIntegerShort4Max |  18.504 ns |   0.6868 ns |  0.0388 ns | 0.0057 |      24 B |
|            WriteInteger8Zero |  15.894 ns |   2.4368 ns |  0.1377 ns | 0.0057 |      24 B |
|             WriteInteger8Max |  28.697 ns |   3.6754 ns |  0.2077 ns | 0.0057 |      24 B |
|              WriteLong18Zero |  20.933 ns |   1.3638 ns |  0.0771 ns | 0.0057 |      24 B |
|               WriteLong18Max |  51.400 ns |   3.2784 ns |  0.1852 ns | 0.0057 |      24 B |
|            WriteDecimal8Zero |  33.915 ns |   1.2151 ns |  0.0687 ns | 0.0171 |      72 B |
|             WriteDecimal8Max |  95.451 ns |   2.7782 ns |  0.1570 ns | 0.0170 |      72 B |
|           WriteDecimal18Zero |  40.238 ns |   0.9040 ns |  0.0511 ns | 0.0171 |      72 B |
|            WriteDecimal18Max | 253.925 ns |  23.7586 ns |  1.3424 ns | 0.0167 |      72 B |
|               WriteDateTime8 |  52.601 ns |   1.0960 ns |  0.0619 ns | 0.0057 |      24 B |
|              WriteDateTime14 |  76.149 ns |  14.6172 ns |  0.8259 ns | 0.0056 |      24 B |
|              WriteDateTime17 | 103.888 ns |   7.1657 ns |  0.4049 ns | 0.0056 |      24 B |

WriteDecimal18Max is later than WriteNumberTextDecimal18Max, but it improves in `.NET Core 2.1`.

## Map by Attribute

```csharp
[Map(32, NullFiller = (byte)'_')]
[TypeEncoding(932)]
[TypeTrueValue((byte)'Y')]
[TypeFalseValue((byte)'N')]
public class Data
{
    [MapInteger(0, 8)]
    public int IntValue { get; set; }

    [MapDecimal(8, 10, 2, GroupingSize = 3)]
    public int DecimalValue { get; set; }

    [MapText(18, 10)]
    public string StringValue { get; set; }

    [MapBoolean(28)]
    public bool BoolValue1 { get; set; }

    [MapBoolean(29, NullValue = (byte)'-')]
    public bool? BoolValue2 { get; set; }
}
```

```csharp
var mapperFactory = new MapperFactoryConfig()
    .CreateMapByAttribute<Data>()
    .ToMapperFactory();
```

## Map by Expression

```csharp
public class Data
{
    public int IntValue { get; set; }

    public int DecimalValue { get; set; }

    public string StringValue { get; set; }

    public bool BoolValue1 { get; set; }

    public bool? BoolValue2 { get; set; }
}
```

```csharp
var mapperFactory = new MapperFactoryConfig()
    .CreateMapByExpression<Data>(32, config => config
        .NullFiller((byte)'_')
        .TypeEncoding(Encoding.GetEncoding(932))
        .TypeTrueValue((byte)'Y')
        .TypeTrueValue((byte)'N')
        .ForMember(x => x.IntValue, m => m.Integer(8))
        .ForMember(x => x.DecimalValue, m => m.Decimal(10, 2).GroupingSize(3))
        .ForMember(x => x.StringValue, m => m.Text(10))
        .ForMember(x => x.BoolValue1, m => m.Boolean())
        .ForMember(x => x.BoolValue2, m => m.Boolean().Null((byte)'-')))
    .ToMapperFactory();
```

## Converters

| Converter             | Option | Supported Types                                          | Memo                                               |
|-----------------------|--------|----------------------------------------------------------|----------------------------------------------------|
| ArrayConverter        |        | T[]                                                      | Use with other converters                          |
| BinaryCinverter       |        | int, long, short                                         | Support little endian and big endian               |
| BooleanConverter      |        | bool, bool?                                              | Byte value to boolean                              |
| ByteConverter         |        | byte                                                     | Use byte as it is                                  |
| BytesConverter        |        | byte[]                                                   | Simple byte array copy                             |
| DateTimeTextConverter |        | DateTime, DateTime?, DateTimeOffset, DateTimeOffset?     | Using Encoding and default format and parse method |
| NumberTextConverter   |        | int, int?, long, long?, short, short?, decimal, decimal? | Using Encoding and default format and parse method |
| TextConverter         |        | string                                                   | Using Encoding.GetString()/Encoding.GetBytes()     |
| AsciiConverter        | ✅     | string                                                   | Using custom convert for ascii bytes               |
| DateTimeConverter     | ✅     | DateTime, DateTime?, DateTimeOffset, DateTimeOffset?     | Using custom format and parse method               |
| DecimalConverter      | ✅     | decimal, decimal?                                        | Using custom format and parse method               |
| IntegerConverter      | ✅     | int, int?, long, long?, short, short?                    | Using custom format and parse method               |

## Configuration

Use the following method to set the default parameters.

| Method                          | Option | Default value                | Memo                                                       |
|---------------------------------|--------|------------------------------|------------------------------------------------------------|
| DefaultDelimiter()              |        | 0x0D, 0x0A                   | Record delimiter                                           |
| DefaultEncoding()               |        | Encoding.ASCII               | Used in TextConverter                                      |
| DefaultTrim()                   |        | true                         | Used in TextConverter, AsciiConverter, NumberTextConverter |
| DefaultTextPadding()            |        | Padding.Right                | Used in TextConverter, AsciiConverter                      |
| DefaultFiller()                 |        | 0x20                         | Used in various converters                                 |
| DefaultTextFiller()             |        | 0x20                         | Used in TextConverter, AsciiConverter                      |
| DefaultEndian()                 |        | Endian.Big                   | Used in BinaryConverter                                    |
| DefaultTrueValue()              |        | 0x31                         | Used in BooleanConverter                                   |
| DefaultFalseValue()             |        | 0x30                         | Used in BooleanConverter                                   |
| DefaultDateTimeTextEncoding()   |        | Encoding.ASCII               | Used in DateTimeTextConverter                              |
| DefaultDateTimeTextProvider()   |        | CultureInfo.InvariantCulture | Used in DateTimeTextConverter                              |
| DefaultDateTimeTextStyle()      |        | DateTimeStyles.None          | Used in DateTimeTextConverter                              |
| DefaultNumberTextEncoding()     |        | Encoding.ASCII               | Used in NumberTextConverter                                |
| DefaultNumberTextProvider()     |        | CultureInfo.InvariantCulture | Used in NumberTextConverter                                |
| DefaultNumberTextNumberStyle()  |        | NumberStyles.Integer         | Used in NumberTextConverter                                |
| DefaultNumberTextDecimalStyle() |        | NumberStyles.Number          | Used in NumberTextConverter                                |
| DefaultNumberTextPadding()      |        | Padding.Left                 | Used in NumberTextConverter                                |
| DefaultNumberTextFiller()       |        | 0x20                         | Used in NumberTextConverter                                |
| DefaultNumberPadding()          | ✅     | Padding.Left                 | Used in NumberConverter, DecimalConverter                  |
| DefaultZeroFill()               | ✅     | false                        | Used in NumberConverter, DecimalConverter                  |
| DefaultUseGrouping()            | ✅     | false                        | Used in DecimalConverter                                   |
| DefaultNumberFiller()           | ✅     | 0x20                         | Used in NumberConverter, DecimalConverter                  |
| DefaultDateTimeKind()           | ✅     | DateTimeKind.Unspecified     | Used in DateTimeConverter                                  |
## Profile

```csharp
var mapperFactory = new MapperFactoryConfig()
    .CreateMapByExpression<SampleData>(59, c => c
        .ForMember(x => x.Code, m => m.Ascii(13))
        .ForMember(x => x.Name, m => m.Text(20))
        .ForMember(x => x.Qty, m => m.Integer(6))
        .ForMember(x => x.Price, m => m.Decimal(10, 2))
        .ForMember(x => x.Date, m => m.DateTime("yyyyMMdd")))
    .CreateMapByExpression<SampleData>("short", 35, c => c
        .ForMember(x => x.Code, m => m.Ascii(13))
        .ForMember(x => x.Name, m => m.Text(20)))
    .ToMapperFactory();

// default profile
var mapper1 = mapperFactory.Create<SampleData>();

// short profile
var mapper2 = mapperFactory.Create<SampleData>("short");
```

## ASP.NET Core integration

```csharp
// Add formatter in Startup
services.AddMvc(options =>
{
    var config = new ByteMapperFormatterConfig { MapperFactory = mapperFactory };
    config.SupportedMediaTypes.Add("text/x-fixedrecord");
    options.OutputFormatters.Add(new ByteMapperOutputFormatter(config));
    options.InputFormatters.Add(new ByteMapperInputFormatter(config));
});
```

```csharp
// Controller method
[Produces("text/x-fixedrecord")]
[HttpGet]
public SampleData[] GetArray()
{
    return new SampleData[] { ... };
}

[Produces("text/x-fixedrecord")]
[HttpGet]
public SampleData GetSingle()
{
    return new SampleData { ... };
}

[Produces("text/x-fixedrecord")]
[ByteMapperProfile("short")]
[HttpGet]
public SampleData[] GetProfileArray()
{
    return new SampleData[] { ... };
}

[HttpPost]
public IActionResult PostArray([FromBody] SampleData[] values)
{
    return Ok();
}
```

## Implement custom converter

### Converter

Implement `IMapConverter`.

```csharp
public interface IMapConverter
{
    object Read(byte[] buffer, int index);

    void Write(byte[] buffer, int index, object value);
}
```

* Example

```csharp
internal sealed class MyBooleanConverter : IMapConverter
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
```

### ConverterBuilder

Implement `IMapConverterBuilder`.

```csharp
public interface IMapConverterBuilder
{
    bool Match(Type type);

    int CalcSize(Type type);

    IMapConverter CreateConverter(IBuilderContext context, Type type);
}
```

Helper class AbstractMapConverterBuilder is prepared, it is used as follows.

* Example

```csharp
public sealed class MyBooleanConverterBuilder : AbstractMapConverterBuilder<BooleanConverterBuilder>
{
    public byte? TrueValue { get; set; }

    public byte? FalseValue { get; set; }

    static BooleanConverterBuilder()
    {
        AddEntry(typeof(bool), 1, (b, t, c) => b.CreateBooleanConverter(c));
    }

    private IMapConverter CreateBooleanConverter(IBuilderContext context)
    {
        return new MyBooleanConverter(
            TrueValue ?? context.GetParameter<byte>(MyParameter.TrueValue),
            FalseValue ?? context.GetParameter<byte>(MyParameter.FalseValue));
    }
}
```

### For Attribute

Create a derived class of AbstractMemberMapAttribute that wraps Builder.

* Example

```csharp
public sealed class MapMyBooleanAttribute : AbstractMemberMapAttribute
{
    private readonly MyBooleanConverterBuilder builder = new MyBooleanConverterBuilder();

    public byte TrueValue
    {
        get => throw new NotSupportedException();
        set => builder.TrueValue = value;
    }

    public byte FalseValue
    {
        get => throw new NotSupportedException();
        set => builder.FalseValue = value;
    }

    public MapMyBooleanAttribute(int offset)
        : base(offset)
    {
    }

    public override IMapConverterBuilder GetConverterBuilder()
    {
        return builder;
    }
}
```

### For Expressions

Implement `IMemberMapExpression`.

```csharp
public interface IMemberMapExpression
{
    IMapConverterBuilder GetMapConverterBuilder();
}
```

* Example

Declare the Syntax interface for the option and implement it with `IMemberMapExpression`.

```csharp
public interface IMapMyBooleanSyntax
{
    IMapMyBooleanSyntax True(byte value);

    IMapMyBooleanSyntax False(byte value);
}

internal sealed class MapMyBooleanExpression : IMemberMapExpression, IMapMyBooleanSyntax
{
    private readonly MyBooleanConverterBuilder builder = new MyBooleanConverterBuilder();

    public IMapMyBooleanSyntax True(byte value)
    {
        builder.TrueValue = value;
        return this;
    }

    public IMapMyBooleanSyntax False(byte value)
    {
        builder.FalseValue = value;
        return this;
    }

    IMapConverterBuilder IMemberMapExpression.GetMapConverterBuilder()
    {
        return builder;
    }
}
```

Also define extension methods.

```csharp
public static IMapMyBooleanSyntax MyBoolean(this IMemberMapConfigSyntax syntax)
{
    var expression = new MapMyBooleanExpression();
    syntax.Map(expression);
    return expression;
}
```

### Default parameter

If default parameters are used, the following definitions are also prepared.

```csharp
// Parameter definition
public static class MyParameter
{
    public const string TrueValue = nameof(MyParameter) + "." + nameof(TrueValue);

    public const string FalseValue = nameof(MyParameter) + "." + nameof(FalseValue);
}
```

```csharp
// Config default extension method
public static MapperFactoryConfig DefaultTrueValue(this MapperFactoryConfig config, byte value)
{
    return config.AddMyParameter(MyParameter.TrueValue, value);
}

public static MapperFactoryConfig UseMyOptionsDefault(this MapperFactoryConfig config)
{
    this.DefaultTrueValue(0x31);
    this.DefaultFalseValue(0x30);
    return config;
}
```

```csharp
// Type default config for map by attribute
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class TypeTrueValueAttribute : Attribute, ITypeDefaultAttribute
{
    public string Key => MyParameter.TrueValue;

    public object Value { get; }

    public TypeTrueValueAttribute(byte value)
    {
        Value = value;
    }
}
```

```csharp
// Type default config for map by expressions
public static ITypeConfigSyntax<T> TypeTrueValue<T>(this ITypeConfigSyntax<T> syntax, byte value)
{
    return syntax.TypeDefault(MyParameter.TrueValue, value);
}
```

## Future

* Performance improvements using Span.
* Add over 19 digits decimal support to DecimalCoverter using BigInteger.
* Add Double and Float support to DecimalCoverter.
* Add fast UTF-16 converter support.
