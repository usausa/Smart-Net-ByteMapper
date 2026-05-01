# Smart.IO.ByteMapper .NET - byte array mapper library for .NET

[![NuGet](https://img.shields.io/nuget/v/Usa.Smart.IO.ByteMapper.svg)](https://www.nuget.org/packages/Usa.Smart.IO.ByteMapper)

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
public sealed class ComplexData
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
mapper.ToByte(buffer, data);
// buffer
// [XXXXXXXXXXXXXXXXXXXXあああああ                                     1       0       1              1.23          1 20001231235959              \r\n]

// byte array to object
mapper.FromByte(buffer, data);
```

Performance in this case.

* 1,700,000 op/s byte array to object
* 1,150,000 op/s object to byte array

## NuGet

| Package | Note |
|-|-|
| [![NuGet](https://img.shields.io/nuget/v/Usa.Smart.IO.ByteMapper.svg)](https://www.nuget.org/packages/Usa.Smart.IO.ByteMapper/) | Core libyrary |
| [![NuGet](https://img.shields.io/nuget/v/Usa.Smart.IO.ByteMapper.Options.svg)](https://www.nuget.org/packages/Usa.Smart.IO.ByteMapper.Options/) | Optional converters |
| [![NuGet](https://img.shields.io/nuget/v/Usa.Smart.IO.ByteMapper.AspNetCore.svg)](https://www.nuget.org/packages/Usa.Smart.IO.ByteMapper.AspNetCore/) | ASP.NET Core integration |

## Benchmark

``` ini
BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8246/25H2/2025Update/HudsonValley2)
AMD Ryzen 9 5900X 3.70GHz, 1 CPU, 24 logical and 12 physical cores
.NET SDK 10.0.202
  [Host]    : .NET 10.0.6 (10.0.6, 10.0.626.17701), X64 RyuJIT x86-64-v3
  MediumRun : .NET 10.0.6 (10.0.6, 10.0.626.17701), X64 RyuJIT x86-64-v3

Job=MediumRun  IterationCount=15  LaunchCount=2  
WarmupCount=10  
```
| Method                       | Mean       | Error     | StdDev    | Min        | Max        | P90        | Gen0   | Allocated |
|----------------------------- |-----------:|----------:|----------:|-----------:|-----------:|-----------:|-------:|----------:|
| ReadIntBinary                |   2.725 ns | 0.0919 ns | 0.1376 ns |   2.506 ns |   3.038 ns |   2.910 ns | 0.0014 |      24 B |
| ReadBoolean                  |   2.726 ns | 0.0871 ns | 0.1304 ns |   2.509 ns |   3.018 ns |   2.900 ns | 0.0014 |      24 B |
| ReadBytes10                  |   4.235 ns | 0.1344 ns | 0.2011 ns |   3.833 ns |   4.668 ns |   4.517 ns | 0.0024 |      40 B |
| ReadBytes20                  |   4.321 ns | 0.1805 ns | 0.2701 ns |   3.980 ns |   4.957 ns |   4.604 ns | 0.0029 |      48 B |
| ReadSjisText20Single20       |  39.903 ns | 0.7648 ns | 1.1447 ns |  37.995 ns |  41.621 ns |  41.468 ns | 0.0038 |      64 B |
| ReadSjisText20Wide5          |  24.349 ns | 0.4865 ns | 0.7282 ns |  23.197 ns |  25.651 ns |  25.319 ns | 0.0019 |      32 B |
| ReadSjisText20Empty          |  12.053 ns | 0.3067 ns | 0.4590 ns |  11.313 ns |  13.006 ns |  12.677 ns |      - |         - |
| ReadNumberTextShort4Zero     |  17.610 ns | 0.4109 ns | 0.5892 ns |  16.576 ns |  18.794 ns |  18.353 ns | 0.0029 |      48 B |
| ReadNumberTextShort4Max      |  19.535 ns | 0.4547 ns | 0.6806 ns |  18.393 ns |  21.290 ns |  20.255 ns | 0.0033 |      56 B |
| ReadNumberTextInt8Zero       |  18.218 ns | 0.4168 ns | 0.6239 ns |  17.262 ns |  19.836 ns |  18.837 ns | 0.0029 |      48 B |
| ReadNumberTextInt8Max        |  21.825 ns | 0.4868 ns | 0.7136 ns |  20.474 ns |  23.109 ns |  22.770 ns | 0.0038 |      64 B |
| ReadNumberTextLong18Zero     |  21.571 ns | 0.4305 ns | 0.6444 ns |  20.565 ns |  22.673 ns |  22.447 ns | 0.0029 |      48 B |
| ReadNumberTextLong18Max      |  30.081 ns | 0.7795 ns | 1.1667 ns |  28.220 ns |  32.205 ns |  32.013 ns | 0.0052 |      88 B |
| ReadNumberTextDecimal8Zero   |  38.177 ns | 0.9474 ns | 1.3887 ns |  35.983 ns |  40.798 ns |  39.907 ns | 0.0038 |      64 B |
| ReadNumberTextDecimal8Max    |  49.932 ns | 1.5642 ns | 2.3412 ns |  46.281 ns |  54.029 ns |  52.907 ns | 0.0043 |      72 B |
| ReadNumberTextDecimal18Zero  |  41.057 ns | 1.0532 ns | 1.5764 ns |  38.682 ns |  44.929 ns |  42.673 ns | 0.0038 |      64 B |
| ReadNumberTextDecimal18Max   |  79.176 ns | 1.3276 ns | 1.9460 ns |  75.328 ns |  82.757 ns |  81.220 ns | 0.0057 |      96 B |
| ReadNumberTextDecimal28Zero  |  45.133 ns | 0.9511 ns | 1.4236 ns |  42.546 ns |  47.442 ns |  46.709 ns | 0.0038 |      64 B |
| ReadNumberTextDecimal28Max   | 108.821 ns | 3.7502 ns | 5.6132 ns | 100.325 ns | 118.135 ns | 117.475 ns | 0.0066 |     112 B |
| ReadDateTimeText8            |  72.498 ns | 1.9380 ns | 2.9007 ns |  68.179 ns |  78.706 ns |  76.382 ns | 0.0038 |      64 B |
| ReadDateTimeText14           | 103.249 ns | 3.0369 ns | 4.5455 ns |  97.386 ns | 111.632 ns | 109.075 ns | 0.0048 |      80 B |
| ReadDateTimeText17           | 122.014 ns | 3.5737 ns | 5.3490 ns | 116.101 ns | 132.888 ns | 128.332 ns | 0.0046 |      80 B |
| ReadText13Code               |  10.695 ns | 0.3163 ns | 0.4734 ns |   9.767 ns |  11.709 ns |  11.185 ns | 0.0029 |      48 B |
| ReadAscii13Code              |   8.668 ns | 0.3802 ns | 0.5691 ns |   7.877 ns |  10.101 ns |   9.383 ns | 0.0029 |      48 B |
| ReadText30Wide15             |  24.990 ns | 0.8288 ns | 1.2404 ns |  23.552 ns |  27.187 ns |  26.608 ns | 0.0033 |      56 B |
| ReadUnicode30Wide15          |   7.429 ns | 0.1758 ns | 0.2631 ns |   6.978 ns |   7.917 ns |   7.817 ns | 0.0033 |      56 B |
| ReadIntegerShort4Zero        |   4.742 ns | 0.1159 ns | 0.1698 ns |   4.436 ns |   5.129 ns |   4.908 ns | 0.0014 |      24 B |
| ReadIntegerShort4Max         |   5.293 ns | 0.1663 ns | 0.2488 ns |   4.854 ns |   5.675 ns |   5.646 ns | 0.0014 |      24 B |
| ReadInteger8Zero             |   6.438 ns | 0.1584 ns | 0.2371 ns |   6.088 ns |   6.900 ns |   6.740 ns | 0.0014 |      24 B |
| ReadInteger8Max              |   7.194 ns | 0.1185 ns | 0.1774 ns |   6.914 ns |   7.465 ns |   7.420 ns | 0.0014 |      24 B |
| ReadLong18Zero               |  11.542 ns | 0.2254 ns | 0.3374 ns |  10.913 ns |  12.160 ns |  11.858 ns | 0.0014 |      24 B |
| ReadLong18Max                |  12.167 ns | 0.5051 ns | 0.7560 ns |  11.295 ns |  14.036 ns |  13.079 ns | 0.0014 |      24 B |
| ReadDecimal8Zero             |   9.289 ns | 0.1829 ns | 0.2737 ns |   8.849 ns |   9.936 ns |   9.673 ns | 0.0019 |      32 B |
| ReadDecimal8Max              |  11.829 ns | 0.2817 ns | 0.4129 ns |  11.067 ns |  12.625 ns |  12.262 ns | 0.0019 |      32 B |
| ReadDecimal18Zero            |  12.522 ns | 0.3313 ns | 0.4958 ns |  11.571 ns |  13.480 ns |  13.159 ns | 0.0019 |      32 B |
| ReadDecimal18Max             |  18.662 ns | 0.4397 ns | 0.6581 ns |  17.657 ns |  19.922 ns |  19.382 ns | 0.0019 |      32 B |
| ReadDecimal28Zero            |  16.032 ns | 0.5982 ns | 0.8953 ns |  14.667 ns |  17.776 ns |  17.123 ns | 0.0019 |      32 B |
| ReadDecimal28Max             |  36.467 ns | 0.8664 ns | 1.2967 ns |  35.067 ns |  40.056 ns |  38.099 ns | 0.0019 |      32 B |
| ReadDateTime8                |  12.059 ns | 0.2683 ns | 0.4017 ns |  11.478 ns |  12.841 ns |  12.524 ns | 0.0014 |      24 B |
| ReadDateTime14               |  17.416 ns | 0.4510 ns | 0.6750 ns |  16.224 ns |  18.484 ns |  18.164 ns | 0.0014 |      24 B |
| ReadDateTime17               |  19.141 ns | 0.6404 ns | 0.8766 ns |  18.154 ns |  21.625 ns |  19.992 ns | 0.0014 |      24 B |

| Method                       | Mean       | Error     | StdDev    | Min        | Max        | P90        | Gen0   | Allocated |
|----------------------------- |-----------:|----------:|----------:|-----------:|-----------:|-----------:|-------:|----------:|
| WriteIntBinary               |   2.546 ns | 0.0965 ns | 0.1444 ns |   2.303 ns |   2.814 ns |   2.766 ns | 0.0014 |      24 B |
| WriteBoolean                 |   2.959 ns | 0.0803 ns | 0.1201 ns |   2.744 ns |   3.290 ns |   3.110 ns | 0.0014 |      24 B |
| WriteBytes10                 |   2.097 ns | 0.0462 ns | 0.0677 ns |   2.003 ns |   2.276 ns |   2.194 ns |      - |         - |
| WriteBytes20                 |   2.067 ns | 0.0362 ns | 0.0519 ns |   2.012 ns |   2.176 ns |   2.157 ns |      - |         - |
| WriteSjisText20Single20      |  52.436 ns | 1.6011 ns | 2.3469 ns |  49.047 ns |  58.886 ns |  55.684 ns |      - |         - |
| WriteSjisText20Wide5         |  20.204 ns | 0.8861 ns | 1.2988 ns |  18.426 ns |  23.870 ns |  22.079 ns |      - |         - |
| WriteSjisText20Empty         |   9.156 ns | 0.1808 ns | 0.2474 ns |   8.753 ns |   9.777 ns |   9.484 ns |      - |         - |
| WriteNumberTextShort4Zero    |  18.105 ns | 0.5849 ns | 0.8006 ns |  16.873 ns |  19.435 ns |  19.026 ns | 0.0033 |      56 B |
| WriteNumberTextShort4Max     |  25.794 ns | 0.6195 ns | 0.9080 ns |  24.304 ns |  27.750 ns |  27.127 ns | 0.0052 |      88 B |
| WriteNumberTextInt8Zero      |  15.980 ns | 0.2741 ns | 0.3659 ns |  15.381 ns |  17.011 ns |  16.441 ns | 0.0033 |      56 B |
| WriteNumberTextInt8Max       |  29.782 ns | 0.5857 ns | 0.8400 ns |  28.032 ns |  31.136 ns |  30.757 ns | 0.0057 |      96 B |
| WriteNumberTextLong18Zero    |  17.405 ns | 0.3162 ns | 0.4328 ns |  16.648 ns |  18.371 ns |  17.994 ns | 0.0033 |      56 B |
| WriteNumberTextLong18Max     |  45.338 ns | 1.2327 ns | 1.8068 ns |  42.959 ns |  50.571 ns |  47.399 ns | 0.0081 |     136 B |
| WriteNumberTextDecimal8Zero  |  35.935 ns | 0.9331 ns | 1.3080 ns |  33.771 ns |  39.123 ns |  37.565 ns | 0.0052 |      88 B |
| WriteNumberTextDecimal8Max   |  59.423 ns | 1.0473 ns | 1.5351 ns |  56.764 ns |  62.874 ns |  61.237 ns | 0.0067 |     112 B |
| WriteNumberTextDecimal18Zero |  36.922 ns | 0.9417 ns | 1.3804 ns |  34.960 ns |  40.429 ns |  38.530 ns | 0.0052 |      88 B |
| WriteNumberTextDecimal18Max  |  92.708 ns | 1.8747 ns | 2.7479 ns |  88.259 ns |  97.915 ns |  96.804 ns | 0.0085 |     144 B |
| WriteNumberTextDecimal28Zero |  38.159 ns | 0.8131 ns | 1.1399 ns |  36.097 ns |  40.953 ns |  39.482 ns | 0.0052 |      88 B |
| WriteNumberTextDecimal28Max  | 117.806 ns | 2.7362 ns | 4.0106 ns | 111.694 ns | 126.760 ns | 123.794 ns | 0.0100 |     168 B |
| WriteDateTimeText8           |  43.368 ns | 0.9013 ns | 1.2926 ns |  40.726 ns |  46.131 ns |  45.056 ns | 0.0038 |      64 B |
| WriteDateTimeText14          |  62.627 ns | 1.6755 ns | 2.5078 ns |  58.284 ns |  67.399 ns |  66.306 ns | 0.0048 |      80 B |
| WriteDateTimeText17          | 110.218 ns | 2.4851 ns | 3.4017 ns | 105.945 ns | 121.193 ns | 113.566 ns | 0.0048 |      80 B |
| WriteText13Code              |   5.204 ns | 0.0884 ns | 0.1210 ns |   4.989 ns |   5.529 ns |   5.323 ns |      - |         - |
| WriteAscii13Code             |  10.901 ns | 0.1900 ns | 0.2785 ns |  10.486 ns |  11.608 ns |  11.247 ns | 0.0024 |      40 B |
| WriteText30Wide15            |  16.410 ns | 0.3948 ns | 0.5535 ns |  15.469 ns |  17.495 ns |  17.137 ns |      - |         - |
| WriteUnicode30Wide15         |   2.394 ns | 0.0513 ns | 0.0736 ns |   2.297 ns |   2.559 ns |   2.491 ns |      - |         - |
| WriteIntegerShort4Zero       |   6.792 ns | 0.1854 ns | 0.2776 ns |   6.404 ns |   7.387 ns |   7.261 ns | 0.0014 |      24 B |
| WriteIntegerShort4Max        |   8.578 ns | 0.1551 ns | 0.2174 ns |   8.326 ns |   9.079 ns |   8.943 ns | 0.0014 |      24 B |
| WriteInteger8Zero            |  10.847 ns | 0.1960 ns | 0.2873 ns |  10.429 ns |  11.592 ns |  11.159 ns | 0.0014 |      24 B |
| WriteInteger8Max             |  13.279 ns | 0.2347 ns | 0.3366 ns |  12.757 ns |  14.249 ns |  13.763 ns | 0.0014 |      24 B |
| WriteLong18Zero              |  19.980 ns | 0.3717 ns | 0.5449 ns |  19.166 ns |  21.273 ns |  20.734 ns | 0.0014 |      24 B |
| WriteLong18Max               |  29.036 ns | 0.6657 ns | 0.9758 ns |  27.900 ns |  31.949 ns |  30.294 ns | 0.0014 |      24 B |
| WriteDecimal8Zero            |  15.256 ns | 0.2067 ns | 0.2759 ns |  14.852 ns |  16.043 ns |  15.615 ns | 0.0019 |      32 B |
| WriteDecimal8Max             |  36.419 ns | 0.9779 ns | 1.4024 ns |  34.455 ns |  39.777 ns |  37.907 ns | 0.0019 |      32 B |
| WriteDecimal18Zero           |  18.983 ns | 0.4570 ns | 0.6406 ns |  17.634 ns |  20.185 ns |  19.918 ns | 0.0019 |      32 B |
| WriteDecimal18Max            |  58.364 ns | 1.3907 ns | 1.9945 ns |  55.302 ns |  62.671 ns |  61.319 ns | 0.0019 |      32 B |
| WriteDecimal28Zero           |  22.265 ns | 0.6411 ns | 0.8987 ns |  20.677 ns |  24.024 ns |  23.224 ns | 0.0019 |      32 B |
| WriteDecimal28Max            |  80.117 ns | 2.1184 ns | 3.1707 ns |  75.256 ns |  87.191 ns |  84.504 ns | 0.0018 |      32 B |
| WriteDateTime8               |  16.240 ns | 0.4545 ns | 0.6372 ns |  15.606 ns |  17.927 ns |  17.230 ns | 0.0014 |      24 B |
| WriteDateTime14              |  24.997 ns | 0.5338 ns | 0.7825 ns |  23.172 ns |  26.402 ns |  25.847 ns | 0.0014 |      24 B |
| WriteDateTime17              |  26.928 ns | 0.7244 ns | 1.0618 ns |  25.616 ns |  29.498 ns |  28.597 ns | 0.0014 |      24 B |

## Map by Attribute

```csharp
[Map(32, NullFiller = (byte)'_')]
[TypeEncoding(932)]
[TypeTrueValue((byte)'Y')]
[TypeFalseValue((byte)'N')]
public sealed class Data
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
public sealed class Data
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
