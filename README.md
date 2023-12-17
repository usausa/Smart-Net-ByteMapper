# Smart.IO.ByteMapper .NET - byte array mapper library for .NET

[![NuGet Badge](https://buildstats.info/nuget/Usa.Smart.IO.ByteMapper)](https://www.nuget.org/packages/Usa.Smart.IO.ByteMapper/)

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
mapper.ToByte(buffer, 0, data);
// buffer
// [XXXXXXXXXXXXXXXXXXXXあああああ                                     1       0       1              1.23          1 20001231235959              \r\n]

// byte array to object
mapper.FromByte(buffer, 0, data);

```

Performance in this case.

* 1,700,000 op/s byte array to object
* 1,150,000 op/s object to byte array

## NuGet

| Package | Note |
|-|-|
| [![NuGet Badge](https://buildstats.info/nuget/Usa.Smart.IO.ByteMapper)](https://www.nuget.org/packages/Usa.Smart.IO.ByteMapper/) | Core libyrary |
| [![NuGet Badge](https://buildstats.info/nuget/Usa.Smart.IO.ByteMapper.Options)](https://www.nuget.org/packages/Usa.Smart.IO.ByteMapper.Options/) | Optional converters |
| [![NuGet Badge](https://buildstats.info/nuget/Usa.Smart.IO.ByteMapper.AspNetCore)](https://www.nuget.org/packages/Usa.Smart.IO.ByteMapper.AspNetCore/) | ASP.NET Core integration |

## Benchmark

``` ini
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
AMD Ryzen 9 5900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK=6.0.100
  [Host]    : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
  MediumRun : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT

Job=MediumRun  IterationCount=15  LaunchCount=2  
WarmupCount=10  
```

|                       Method |       Mean |     Error |    StdDev |     Median |        Min |        Max |        P90 |  Gen 0 | Allocated |
|----------------------------- |-----------:|----------:|----------:|-----------:|-----------:|-----------:|-----------:|-------:|----------:|
|                ReadIntBinary |   3.345 ns | 0.0774 ns | 0.1159 ns |   3.316 ns |   3.230 ns |   3.572 ns |   3.551 ns | 0.0014 |      24 B |
|                  ReadBoolean |   5.022 ns | 0.0328 ns | 0.0481 ns |   5.033 ns |   4.947 ns |   5.098 ns |   5.085 ns | 0.0014 |      24 B |
|                  ReadBytes10 |   5.874 ns | 0.0190 ns | 0.0266 ns |   5.874 ns |   5.812 ns |   5.925 ns |   5.904 ns | 0.0024 |      40 B |
|                  ReadBytes20 |   5.930 ns | 0.0975 ns | 0.1367 ns |   5.857 ns |   5.781 ns |   6.232 ns |   6.067 ns | 0.0029 |      48 B |
|       ReadSjisText20Single20 |  52.194 ns | 2.2086 ns | 3.1675 ns |  52.198 ns |  48.863 ns |  55.666 ns |  55.373 ns | 0.0038 |      64 B |
|          ReadSjisText20Wide5 |  32.381 ns | 0.5825 ns | 0.8718 ns |  32.353 ns |  31.328 ns |  33.406 ns |  33.350 ns | 0.0019 |      32 B |
|          ReadSjisText20Empty |  16.353 ns | 0.0348 ns | 0.0521 ns |  16.340 ns |  16.273 ns |  16.476 ns |  16.427 ns |      - |         - |
|     ReadNumberTextShort4Zero |  22.510 ns | 0.1019 ns | 0.1461 ns |  22.481 ns |  22.275 ns |  22.772 ns |  22.695 ns | 0.0029 |      48 B |
|      ReadNumberTextShort4Max |  22.668 ns | 0.3600 ns | 0.5388 ns |  22.439 ns |  22.250 ns |  24.294 ns |  23.354 ns | 0.0033 |      56 B |
|       ReadNumberTextInt8Zero |  23.409 ns | 0.2145 ns | 0.3144 ns |  23.223 ns |  22.936 ns |  23.848 ns |  23.785 ns | 0.0029 |      48 B |
|        ReadNumberTextInt8Max |  25.040 ns | 0.0904 ns | 0.1326 ns |  25.066 ns |  24.827 ns |  25.254 ns |  25.190 ns | 0.0038 |      64 B |
|     ReadNumberTextLong18Zero |  27.307 ns | 0.8070 ns | 1.1829 ns |  26.299 ns |  26.039 ns |  29.010 ns |  28.698 ns | 0.0029 |      48 B |
|      ReadNumberTextLong18Max |  32.598 ns | 0.4815 ns | 0.7207 ns |  32.372 ns |  31.771 ns |  34.288 ns |  33.701 ns | 0.0052 |      88 B |
|   ReadNumberTextDecimal8Zero |  41.387 ns | 0.1724 ns | 0.2417 ns |  41.314 ns |  41.043 ns |  41.974 ns |  41.689 ns | 0.0038 |      64 B |
|    ReadNumberTextDecimal8Max |  53.479 ns | 0.1082 ns | 0.1517 ns |  53.491 ns |  53.180 ns |  53.742 ns |  53.663 ns | 0.0043 |      72 B |
|  ReadNumberTextDecimal18Zero |  48.544 ns | 0.2512 ns | 0.3438 ns |  48.488 ns |  47.528 ns |  49.079 ns |  48.949 ns | 0.0038 |      64 B |
|   ReadNumberTextDecimal18Max |  77.345 ns | 0.2930 ns | 0.4385 ns |  77.304 ns |  76.483 ns |  78.244 ns |  77.797 ns | 0.0057 |      96 B |
|  ReadNumberTextDecimal28Zero |  55.038 ns | 1.2192 ns | 1.6688 ns |  55.033 ns |  53.032 ns |  56.810 ns |  56.744 ns | 0.0038 |      64 B |
|   ReadNumberTextDecimal28Max | 104.013 ns | 0.4448 ns | 0.6658 ns | 103.908 ns | 103.065 ns | 105.241 ns | 105.076 ns | 0.0066 |     112 B |
|            ReadDateTimeText8 |  98.251 ns | 0.2435 ns | 0.3644 ns |  98.174 ns |  97.712 ns |  99.111 ns |  98.737 ns | 0.0038 |      64 B |
|           ReadDateTimeText14 | 140.822 ns | 1.1505 ns | 1.7221 ns | 140.893 ns | 138.492 ns | 143.298 ns | 142.794 ns | 0.0046 |      80 B |
|           ReadDateTimeText17 | 161.067 ns | 3.1529 ns | 4.5217 ns | 161.275 ns | 155.872 ns | 166.506 ns | 165.891 ns | 0.0046 |      80 B |
|               ReadText13Code |  14.578 ns | 0.0250 ns | 0.0367 ns |  14.577 ns |  14.524 ns |  14.646 ns |  14.626 ns | 0.0029 |      48 B |
|              ReadAscii13Code |  10.305 ns | 0.2965 ns | 0.4346 ns |  10.635 ns |   9.792 ns |  10.872 ns |  10.780 ns | 0.0029 |      48 B |
|             ReadText30Wide15 |  30.066 ns | 0.0541 ns | 0.0792 ns |  30.062 ns |  29.949 ns |  30.206 ns |  30.173 ns | 0.0033 |      56 B |
|          ReadUnicode30Wide15 |   8.543 ns | 0.4213 ns | 0.6042 ns |   8.905 ns |   7.877 ns |   9.344 ns |   9.172 ns | 0.0033 |      56 B |
|        ReadIntegerShort4Zero |   5.962 ns | 0.0647 ns | 0.0969 ns |   5.943 ns |   5.829 ns |   6.225 ns |   6.063 ns | 0.0014 |      24 B |
|         ReadIntegerShort4Max |   5.540 ns | 0.0522 ns | 0.0782 ns |   5.509 ns |   5.457 ns |   5.690 ns |   5.672 ns | 0.0014 |      24 B |
|             ReadInteger8Zero |   6.964 ns | 0.0525 ns | 0.0736 ns |   6.946 ns |   6.866 ns |   7.066 ns |   7.055 ns | 0.0014 |      24 B |
|              ReadInteger8Max |   7.221 ns | 0.0165 ns | 0.0232 ns |   7.221 ns |   7.185 ns |   7.284 ns |   7.246 ns | 0.0014 |      24 B |
|               ReadLong18Zero |  10.934 ns | 0.9761 ns | 1.4610 ns |  11.047 ns |   9.148 ns |  12.452 ns |  12.412 ns | 0.0014 |      24 B |
|                ReadLong18Max |  12.429 ns | 0.5720 ns | 0.8385 ns |  12.467 ns |  11.590 ns |  13.591 ns |  13.562 ns | 0.0014 |      24 B |
|             ReadDecimal8Zero |  13.540 ns | 0.0273 ns | 0.0401 ns |  13.539 ns |  13.455 ns |  13.626 ns |  13.599 ns | 0.0019 |      32 B |
|              ReadDecimal8Max |  15.916 ns | 0.2583 ns | 0.3786 ns |  15.776 ns |  15.613 ns |  16.761 ns |  16.709 ns | 0.0019 |      32 B |
|            ReadDecimal18Zero |  15.285 ns | 0.0502 ns | 0.0736 ns |  15.287 ns |  15.147 ns |  15.422 ns |  15.377 ns | 0.0019 |      32 B |
|             ReadDecimal18Max |  24.128 ns | 0.1211 ns | 0.1737 ns |  24.162 ns |  23.854 ns |  24.401 ns |  24.366 ns | 0.0019 |      32 B |
|            ReadDecimal28Zero |  18.241 ns | 0.0428 ns | 0.0627 ns |  18.249 ns |  18.096 ns |  18.366 ns |  18.311 ns | 0.0019 |      32 B |
|             ReadDecimal28Max |  45.839 ns | 0.1844 ns | 0.2759 ns |  45.871 ns |  45.398 ns |  46.284 ns |  46.199 ns | 0.0019 |      32 B |
|                ReadDateTime8 |  15.217 ns | 0.0831 ns | 0.1218 ns |  15.156 ns |  15.050 ns |  15.420 ns |  15.401 ns | 0.0014 |      24 B |
|               ReadDateTime14 |  22.269 ns | 0.0972 ns | 0.1394 ns |  22.255 ns |  22.033 ns |  22.502 ns |  22.435 ns | 0.0014 |      24 B |
|               ReadDateTime17 |  30.346 ns | 4.4469 ns | 6.2339 ns |  24.572 ns |  24.269 ns |  36.855 ns |  36.761 ns | 0.0014 |      24 B |

|                       Method |       Mean |     Error |    StdDev |     Median |        Min |        Max |        P90 |  Gen 0 | Allocated |
|----------------------------- |-----------:|----------:|----------:|-----------:|-----------:|-----------:|-----------:|-------:|----------:|
|               WriteIntBinary |   3.655 ns | 0.0756 ns | 0.1085 ns |   3.572 ns |   3.539 ns |   3.788 ns |   3.778 ns | 0.0014 |      24 B |
|                 WriteBoolean |   3.574 ns | 0.0213 ns | 0.0312 ns |   3.570 ns |   3.521 ns |   3.651 ns |   3.619 ns | 0.0014 |      24 B |
|                 WriteBytes10 |   5.724 ns | 0.0081 ns | 0.0122 ns |   5.722 ns |   5.704 ns |   5.750 ns |   5.741 ns |      - |         - |
|                 WriteBytes20 |   5.530 ns | 0.0092 ns | 0.0128 ns |   5.530 ns |   5.508 ns |   5.570 ns |   5.541 ns |      - |         - |
|      WriteSjisText20Single20 |  78.005 ns | 0.1827 ns | 0.2678 ns |  78.046 ns |  77.575 ns |  78.468 ns |  78.314 ns | 0.0028 |      48 B |
|         WriteSjisText20Wide5 |  34.718 ns | 0.0794 ns | 0.1164 ns |  34.723 ns |  34.472 ns |  35.016 ns |  34.850 ns | 0.0024 |      40 B |
|         WriteSjisText20Empty |  27.137 ns | 0.0821 ns | 0.1177 ns |  27.099 ns |  26.996 ns |  27.483 ns |  27.283 ns | 0.0033 |      56 B |
|    WriteNumberTextShort4Zero |  19.482 ns | 0.0531 ns | 0.0779 ns |  19.487 ns |  19.310 ns |  19.637 ns |  19.575 ns | 0.0033 |      56 B |
|     WriteNumberTextShort4Max |  24.889 ns | 0.1000 ns | 0.1402 ns |  24.890 ns |  24.636 ns |  25.074 ns |  25.047 ns | 0.0052 |      88 B |
|      WriteNumberTextInt8Zero |  20.650 ns | 0.0463 ns | 0.0649 ns |  20.648 ns |  20.539 ns |  20.802 ns |  20.741 ns | 0.0033 |      56 B |
|       WriteNumberTextInt8Max |  27.394 ns | 0.0648 ns | 0.0970 ns |  27.395 ns |  27.137 ns |  27.539 ns |  27.528 ns | 0.0057 |      96 B |
|    WriteNumberTextLong18Zero |  24.486 ns | 0.0733 ns | 0.1051 ns |  24.499 ns |  24.296 ns |  24.687 ns |  24.598 ns | 0.0033 |      56 B |
|     WriteNumberTextLong18Max |  40.001 ns | 0.1535 ns | 0.2298 ns |  39.962 ns |  39.614 ns |  40.531 ns |  40.260 ns | 0.0081 |     136 B |
|  WriteNumberTextDecimal8Zero |  43.059 ns | 0.1351 ns | 0.1937 ns |  43.050 ns |  42.661 ns |  43.400 ns |  43.345 ns | 0.0052 |      88 B |
|   WriteNumberTextDecimal8Max |  62.609 ns | 0.2993 ns | 0.4388 ns |  62.672 ns |  61.817 ns |  63.215 ns |  63.157 ns | 0.0066 |     112 B |
| WriteNumberTextDecimal18Zero |  45.796 ns | 0.1153 ns | 0.1690 ns |  45.808 ns |  45.393 ns |  46.100 ns |  45.989 ns | 0.0052 |      88 B |
|  WriteNumberTextDecimal18Max |  91.072 ns | 1.8825 ns | 2.8177 ns |  91.233 ns |  87.864 ns |  94.347 ns |  94.058 ns | 0.0085 |     144 B |
| WriteNumberTextDecimal28Zero |  49.022 ns | 0.1017 ns | 0.1426 ns |  49.018 ns |  48.593 ns |  49.263 ns |  49.180 ns | 0.0052 |      88 B |
|  WriteNumberTextDecimal28Max | 126.365 ns | 0.5301 ns | 0.7602 ns | 126.623 ns | 124.858 ns | 127.465 ns | 127.173 ns | 0.0100 |     168 B |
|           WriteDateTimeText8 |  85.799 ns | 0.6347 ns | 0.9304 ns |  85.524 ns |  84.353 ns |  87.199 ns |  86.820 ns | 0.0057 |      96 B |
|          WriteDateTimeText14 | 120.636 ns | 0.6155 ns | 0.9212 ns | 120.716 ns | 119.015 ns | 122.145 ns | 121.706 ns | 0.0071 |     120 B |
|          WriteDateTimeText17 | 210.494 ns | 0.5407 ns | 0.7754 ns | 210.300 ns | 209.300 ns | 212.644 ns | 211.284 ns | 0.0076 |     128 B |
|              WriteText13Code |  16.510 ns | 0.0571 ns | 0.0837 ns |  16.500 ns |  16.350 ns |  16.698 ns |  16.625 ns | 0.0024 |      40 B |
|             WriteAscii13Code |  11.706 ns | 0.0357 ns | 0.0524 ns |  11.706 ns |  11.630 ns |  11.819 ns |  11.768 ns | 0.0024 |      40 B |
|            WriteText30Wide15 |  26.997 ns | 0.0664 ns | 0.0952 ns |  26.991 ns |  26.841 ns |  27.233 ns |  27.104 ns | 0.0033 |      56 B |
|         WriteUnicode30Wide15 |   4.468 ns | 0.0094 ns | 0.0122 ns |   4.466 ns |   4.451 ns |   4.489 ns |   4.485 ns |      - |         - |
|       WriteIntegerShort4Zero |   8.110 ns | 0.2074 ns | 0.3040 ns |   7.865 ns |   7.784 ns |   8.469 ns |   8.431 ns | 0.0014 |      24 B |
|        WriteIntegerShort4Max |  10.172 ns | 0.1290 ns | 0.1891 ns |  10.274 ns |   9.931 ns |  10.420 ns |  10.390 ns | 0.0014 |      24 B |
|            WriteInteger8Zero |   9.564 ns | 0.1997 ns | 0.2864 ns |   9.549 ns |   9.209 ns |   9.929 ns |   9.874 ns | 0.0014 |      24 B |
|             WriteInteger8Max |  13.802 ns | 0.0361 ns | 0.0530 ns |  13.804 ns |  13.710 ns |  13.888 ns |  13.869 ns | 0.0014 |      24 B |
|              WriteLong18Zero |  11.346 ns | 0.0647 ns | 0.0907 ns |  11.390 ns |  11.192 ns |  11.470 ns |  11.439 ns | 0.0014 |      24 B |
|               WriteLong18Max |  24.592 ns | 0.0493 ns | 0.0722 ns |  24.597 ns |  24.423 ns |  24.758 ns |  24.668 ns | 0.0014 |      24 B |
|            WriteDecimal8Zero |  17.599 ns | 0.0534 ns | 0.0799 ns |  17.584 ns |  17.446 ns |  17.816 ns |  17.680 ns | 0.0043 |      72 B |
|             WriteDecimal8Max |  37.087 ns | 0.5286 ns | 0.7748 ns |  37.531 ns |  36.052 ns |  38.028 ns |  37.967 ns | 0.0043 |      72 B |
|           WriteDecimal18Zero |  19.800 ns | 0.0576 ns | 0.0844 ns |  19.809 ns |  19.653 ns |  19.950 ns |  19.904 ns | 0.0043 |      72 B |
|            WriteDecimal18Max |  55.634 ns | 0.0986 ns | 0.1446 ns |  55.635 ns |  55.279 ns |  55.912 ns |  55.788 ns | 0.0043 |      72 B |
|           WriteDecimal28Zero |  22.239 ns | 0.1084 ns | 0.1484 ns |  22.247 ns |  21.996 ns |  22.479 ns |  22.434 ns | 0.0043 |      72 B |
|            WriteDecimal28Max |  77.243 ns | 0.3443 ns | 0.5047 ns |  77.411 ns |  76.368 ns |  78.150 ns |  77.866 ns | 0.0043 |      72 B |
|               WriteDateTime8 |  18.810 ns | 0.0582 ns | 0.0853 ns |  18.811 ns |  18.660 ns |  18.963 ns |  18.906 ns | 0.0014 |      24 B |
|              WriteDateTime14 |  27.737 ns | 0.2297 ns | 0.3294 ns |  27.760 ns |  27.296 ns |  28.201 ns |  28.090 ns | 0.0014 |      24 B |
|              WriteDateTime17 |  31.469 ns | 0.0764 ns | 0.1096 ns |  31.472 ns |  31.280 ns |  31.674 ns |  31.600 ns | 0.0014 |      24 B |

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
