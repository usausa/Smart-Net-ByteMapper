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

* 1,650,000 op/s byte array to object
* 1,150,000 op/s object to byte array

## NuGet

| Id                                 | Note                       |
|------------------------------------|----------------------------|
| Usa.Smart.IO.ByteMapper            | Core libyrary              |
| Usa.Smart.IO.ByteMapper.Options    | Optional converters        |
| Usa.Smart.IO.ByteMapper.AspNetCore | ASP.NET Core integration   |

## Benchmark

Converter benchmark on Intel Core i7-4771 CPU 3.50GHz (Haswell) / .NET Core SDK=2.1.200.

|                       Method |       Mean |     Error |    StdDev |     Median |  Gen 0 | Allocated |
|----------------------------- |-----------:|----------:|----------:|-----------:|-------:|----------:|
|                ReadIntBinary |   6.008 ns | 0.1048 ns | 0.1537 ns |   6.022 ns | 0.0057 |      24 B |
|                  ReadBoolean |   4.535 ns | 0.0125 ns | 0.0187 ns |   4.535 ns | 0.0057 |      24 B |
|                  ReadBytes10 |   9.427 ns | 0.0245 ns | 0.0360 ns |   9.428 ns | 0.0095 |      40 B |
|                  ReadBytes20 |   9.844 ns | 0.0216 ns | 0.0310 ns |   9.846 ns | 0.0114 |      48 B |
|       ReadSjisText20Single20 | 109.078 ns | 0.2585 ns | 0.3708 ns | 109.017 ns | 0.0169 |      72 B |
|          ReadSjisText20Wide5 |  74.322 ns | 0.2286 ns | 0.3421 ns |  74.324 ns | 0.0094 |      40 B |
|          ReadSjisText20Empty |  36.377 ns | 0.0800 ns | 0.1173 ns |  36.399 ns |      - |       0 B |
|     ReadNumberTextShort4Zero |  93.764 ns | 0.2262 ns | 0.3097 ns |  93.755 ns | 0.0132 |      56 B |
|      ReadNumberTextShort4Max | 112.630 ns | 0.1952 ns | 0.2800 ns | 112.623 ns | 0.0151 |      64 B |
|       ReadNumberTextInt8Zero |  94.275 ns | 0.3721 ns | 0.5093 ns |  94.179 ns | 0.0132 |      56 B |
|        ReadNumberTextInt8Max | 132.544 ns | 0.3048 ns | 0.4562 ns | 132.530 ns | 0.0172 |      72 B |
|     ReadNumberTextLong18Zero |  97.926 ns | 0.1763 ns | 0.2584 ns |  97.890 ns | 0.0132 |      56 B |
|      ReadNumberTextLong18Max | 191.818 ns | 0.3740 ns | 0.5482 ns | 191.830 ns | 0.0207 |      88 B |
|   ReadNumberTextDecimal8Zero | 109.968 ns | 0.1422 ns | 0.2085 ns | 109.989 ns | 0.0151 |      64 B |
|    ReadNumberTextDecimal8Max | 218.592 ns | 0.3121 ns | 0.4477 ns | 218.509 ns | 0.0188 |      80 B |
|  ReadNumberTextDecimal18Zero | 126.334 ns | 0.3040 ns | 0.4457 ns | 126.197 ns | 0.0169 |      72 B |
|   ReadNumberTextDecimal18Max | 406.023 ns | 0.5058 ns | 0.7414 ns | 406.113 ns | 0.0224 |      96 B |
|  ReadNumberTextDecimal28Zero | 136.724 ns | 0.2520 ns | 0.3694 ns | 136.731 ns | 0.0169 |      72 B |
|   ReadNumberTextDecimal28Max | 608.510 ns | 0.7258 ns | 1.0409 ns | 608.514 ns | 0.0277 |     120 B |
|            ReadDateTimeText8 | 250.909 ns | 0.3845 ns | 0.5756 ns | 251.035 ns | 0.0172 |      72 B |
|           ReadDateTimeText14 | 360.620 ns | 1.1534 ns | 1.6907 ns | 360.587 ns | 0.0186 |      80 B |
|           ReadDateTimeText17 | 450.441 ns | 0.5733 ns | 0.8581 ns | 450.307 ns | 0.0205 |      88 B |
|               ReadText13Code |  32.808 ns | 0.0417 ns | 0.0625 ns |  32.807 ns | 0.0133 |      56 B |
|              ReadAscii13Code |  17.528 ns | 0.0226 ns | 0.0317 ns |  17.523 ns | 0.0133 |      56 B |
|             ReadText30Wide15 | 116.288 ns | 0.2416 ns | 0.3387 ns | 116.357 ns | 0.0132 |      56 B |
|          ReadUnicode30Wide15 |  13.595 ns | 0.0810 ns | 0.1187 ns |  13.648 ns | 0.0133 |      56 B |
|        ReadIntegerShort4Zero |  11.045 ns | 0.1059 ns | 0.1552 ns |  11.033 ns | 0.0057 |      24 B |
|         ReadIntegerShort4Max |  13.980 ns | 0.0886 ns | 0.1241 ns |  14.002 ns | 0.0057 |      24 B |
|             ReadInteger8Zero |  12.118 ns | 0.0328 ns | 0.0481 ns |  12.129 ns | 0.0057 |      24 B |
|              ReadInteger8Max |  18.881 ns | 0.0304 ns | 0.0435 ns |  18.868 ns | 0.0057 |      24 B |
|               ReadLong18Zero |  16.771 ns | 0.0954 ns | 0.1398 ns |  16.806 ns | 0.0057 |      24 B |
|                ReadLong18Max |  32.234 ns | 0.0754 ns | 0.1057 ns |  32.246 ns | 0.0057 |      24 B |
|             ReadDecimal8Zero |  23.713 ns | 0.0599 ns | 0.0878 ns |  23.699 ns | 0.0076 |      32 B |
|              ReadDecimal8Max |  28.658 ns | 0.0794 ns | 0.1189 ns |  28.651 ns | 0.0076 |      32 B |
|            ReadDecimal18Zero |  29.092 ns | 0.0780 ns | 0.1119 ns |  29.093 ns | 0.0076 |      32 B |
|             ReadDecimal18Max |  45.289 ns | 0.0941 ns | 0.1380 ns |  45.294 ns | 0.0076 |      32 B |
|            ReadDecimal28Zero |  35.645 ns | 0.0624 ns | 0.0766 ns |  35.641 ns | 0.0076 |      32 B |
|             ReadDecimal28Max |  88.290 ns | 0.2513 ns | 0.3762 ns |  88.285 ns | 0.0075 |      32 B |
|                ReadDateTime8 |  31.013 ns | 0.0658 ns | 0.0964 ns |  31.014 ns | 0.0057 |      24 B |
|               ReadDateTime14 |  43.595 ns | 0.1259 ns | 0.1846 ns |  43.603 ns | 0.0057 |      24 B |
|               ReadDateTime17 |  48.595 ns | 0.1176 ns | 0.1761 ns |  48.583 ns | 0.0057 |      24 B |

|                       Method |       Mean |     Error |    StdDev |     Median |  Gen 0 | Allocated |
|----------------------------- |-----------:|----------:|----------:|-----------:|-------:|----------:|
|               WriteIntBinary |   5.563 ns | 0.0312 ns | 0.0437 ns |   5.560 ns | 0.0057 |      24 B |
|                 WriteBoolean |   4.889 ns | 0.0256 ns | 0.0367 ns |   4.902 ns | 0.0057 |      24 B |
|                 WriteBytes10 |   9.441 ns | 0.0193 ns | 0.0289 ns |   9.440 ns |      - |       0 B |
|                 WriteBytes20 |   9.685 ns | 0.0317 ns | 0.0455 ns |   9.682 ns |      - |       0 B |
|      WriteSjisText20Single20 | 142.690 ns | 0.2519 ns | 0.3532 ns | 142.555 ns | 0.0112 |      48 B |
|         WriteSjisText20Wide5 |  76.882 ns | 0.2229 ns | 0.3196 ns |  76.906 ns | 0.0094 |      40 B |
|         WriteSjisText20Empty |  64.721 ns | 0.2657 ns | 0.3895 ns |  64.745 ns | 0.0132 |      56 B |
|    WriteNumberTextShort4Zero |  87.116 ns | 0.3057 ns | 0.4286 ns |  87.223 ns | 0.0209 |      88 B |
|     WriteNumberTextShort4Max |  93.377 ns | 0.2232 ns | 0.3272 ns |  93.271 ns | 0.0228 |      96 B |
|      WriteNumberTextInt8Zero |  81.055 ns | 0.1109 ns | 0.1590 ns |  81.066 ns | 0.0209 |      88 B |
|       WriteNumberTextInt8Max |  97.532 ns | 0.3573 ns | 0.5124 ns |  97.453 ns | 0.0247 |     104 B |
|    WriteNumberTextLong18Zero |  88.653 ns | 0.3954 ns | 0.5671 ns |  88.757 ns | 0.0209 |      88 B |
|     WriteNumberTextLong18Max | 140.757 ns | 0.3734 ns | 0.5588 ns | 140.636 ns | 0.0322 |     136 B |
|  WriteNumberTextDecimal8Zero | 106.846 ns | 0.4619 ns | 0.6475 ns | 106.890 ns | 0.0228 |      96 B |
|   WriteNumberTextDecimal8Max | 141.635 ns | 0.2558 ns | 0.3414 ns | 141.657 ns | 0.0284 |     120 B |
| WriteNumberTextDecimal18Zero | 112.207 ns | 0.2258 ns | 0.3090 ns | 112.102 ns | 0.0228 |      96 B |
|  WriteNumberTextDecimal18Max | 198.005 ns | 0.6524 ns | 0.9563 ns | 198.037 ns | 0.0343 |     144 B |
| WriteNumberTextDecimal28Zero | 117.634 ns | 0.1460 ns | 0.2140 ns | 117.566 ns | 0.0228 |      96 B |
|  WriteNumberTextDecimal28Max | 255.708 ns | 0.9047 ns | 1.2683 ns | 255.513 ns | 0.0415 |     176 B |
|           WriteDateTimeText8 | 315.387 ns | 3.8047 ns | 5.5769 ns | 311.087 ns | 0.0491 |     208 B |
|          WriteDateTimeText14 | 388.905 ns | 1.1436 ns | 1.6401 ns | 388.555 ns | 0.0529 |     224 B |
|          WriteDateTimeText17 | 555.873 ns | 4.7009 ns | 6.7419 ns | 558.530 ns | 0.0639 |     272 B |
|              WriteText13Code |  41.750 ns | 0.1660 ns | 0.2381 ns |  41.787 ns | 0.0095 |      40 B |
|             WriteAscii13Code |  26.458 ns | 0.6532 ns | 0.9574 ns |  25.753 ns | 0.0095 |      40 B |
|            WriteText30Wide15 |  54.329 ns | 0.0907 ns | 0.1301 ns |  54.311 ns | 0.0133 |      56 B |
|         WriteUnicode30Wide15 |   7.115 ns | 0.0163 ns | 0.0234 ns |   7.113 ns |      - |       0 B |
|       WriteIntegerShort4Zero |  11.920 ns | 0.0224 ns | 0.0335 ns |  11.922 ns | 0.0057 |      24 B |
|        WriteIntegerShort4Max |  15.058 ns | 0.0263 ns | 0.0385 ns |  15.063 ns | 0.0057 |      24 B |
|            WriteInteger8Zero |  13.473 ns | 0.0443 ns | 0.0636 ns |  13.476 ns | 0.0057 |      24 B |
|             WriteInteger8Max |  20.756 ns | 0.0507 ns | 0.0677 ns |  20.767 ns | 0.0057 |      24 B |
|              WriteLong18Zero |  16.933 ns | 0.1206 ns | 0.1729 ns |  16.926 ns | 0.0057 |      24 B |
|               WriteLong18Max |  36.666 ns | 0.1137 ns | 0.1518 ns |  36.730 ns | 0.0057 |      24 B |
|            WriteDecimal8Zero |  40.011 ns | 0.0646 ns | 0.0946 ns |  39.984 ns | 0.0171 |      72 B |
|             WriteDecimal8Max |  66.303 ns | 0.1839 ns | 0.2637 ns |  66.285 ns | 0.0170 |      72 B |
|           WriteDecimal18Zero |  50.991 ns | 0.4686 ns | 0.6869 ns |  50.774 ns | 0.0171 |      72 B |
|            WriteDecimal18Max | 101.248 ns | 0.5320 ns | 0.7798 ns | 101.086 ns | 0.0170 |      72 B |
|           WriteDecimal28Zero |  68.488 ns | 0.9956 ns | 1.4594 ns |  67.457 ns | 0.0170 |      72 B |
|            WriteDecimal28Max | 139.577 ns | 0.2808 ns | 0.4028 ns | 139.695 ns | 0.0169 |      72 B |
|               WriteDateTime8 |  30.674 ns | 0.1109 ns | 0.1554 ns |  30.701 ns | 0.0057 |      24 B |
|              WriteDateTime14 |  48.465 ns | 0.1791 ns | 0.2681 ns |  48.398 ns | 0.0057 |      24 B |
|              WriteDateTime17 |  55.587 ns | 0.2386 ns | 0.3497 ns |  55.457 ns | 0.0057 |      24 B |

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
        .TypeFalseValue((byte)'N')
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
| BinaryCinverter       |        | int, long, short, double, float                          | Support little endian and big endian               |
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
| DefaultDateTimeKind()           |        | DateTimeKind.Unspecified     | Used in DateTimeConverter                                  |
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

* Add Nullable support to BinaryCoverter.
* Add Guid support to BinaryCoverter(?).
* Add Double and Float support to NumberTextCoverter.
* Add DoubleConverter and FloatConverter.
* Performance improvements using Span.
