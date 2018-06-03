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

* 1,700,000 op/s byte array to object
* 1,150,000 op/s object to byte array

## NuGet

| Id                                 | Note                       |
|------------------------------------|----------------------------|
| Usa.Smart.IO.ByteMapper            | Core libyrary              |
| Usa.Smart.IO.ByteMapper.Options    | Optional converters        |
| Usa.Smart.IO.ByteMapper.AspNetCore | ASP.NET Core integration   |

## Benchmark

Converter benchmark on Intel Core i7-4771 CPU 3.50GHz (Haswell) / .NET Core SDK=2.1.300.

|                       Method |       Mean |     Error |    StdDev |     Median |  Gen 0 | Allocated |
|----------------------------- |-----------:|----------:|----------:|-----------:|-------:|----------:|
|                ReadIntBinary |   6.558 ns | 0.0876 ns | 0.1256 ns |   6.566 ns | 0.0057 |      24 B |
|                  ReadBoolean |   5.430 ns | 0.1348 ns | 0.2018 ns |   5.422 ns | 0.0057 |      24 B |
|                  ReadBytes10 |  10.043 ns | 0.0200 ns | 0.0267 ns |  10.049 ns | 0.0095 |      40 B |
|                  ReadBytes20 |  10.842 ns | 0.1011 ns | 0.1418 ns |  10.790 ns | 0.0114 |      48 B |
|       ReadSjisText20Single20 | 102.357 ns | 2.2108 ns | 3.0992 ns | 101.409 ns | 0.0170 |      72 B |
|          ReadSjisText20Wide5 |  67.208 ns | 0.1745 ns | 0.2388 ns |  67.282 ns | 0.0094 |      40 B |
|          ReadSjisText20Empty |  31.843 ns | 0.2120 ns | 0.3174 ns |  31.814 ns |      - |       0 B |
|     ReadNumberTextShort4Zero |  91.876 ns | 0.4368 ns | 0.6264 ns |  91.725 ns | 0.0132 |      56 B |
|      ReadNumberTextShort4Max | 110.076 ns | 0.3555 ns | 0.5211 ns | 110.114 ns | 0.0151 |      64 B |
|       ReadNumberTextInt8Zero |  92.651 ns | 0.2332 ns | 0.3269 ns |  92.715 ns | 0.0132 |      56 B |
|        ReadNumberTextInt8Max | 131.123 ns | 0.7586 ns | 1.1355 ns | 131.389 ns | 0.0172 |      72 B |
|     ReadNumberTextLong18Zero |  96.182 ns | 0.2780 ns | 0.3897 ns |  96.335 ns | 0.0132 |      56 B |
|      ReadNumberTextLong18Max | 180.025 ns | 0.7178 ns | 1.0522 ns | 179.783 ns | 0.0207 |      88 B |
|   ReadNumberTextDecimal8Zero | 109.047 ns | 0.4831 ns | 0.6449 ns | 109.030 ns | 0.0151 |      64 B |
|    ReadNumberTextDecimal8Max | 206.920 ns | 0.7600 ns | 1.1140 ns | 206.509 ns | 0.0188 |      80 B |
|  ReadNumberTextDecimal18Zero | 124.131 ns | 0.9193 ns | 1.3184 ns | 123.699 ns | 0.0169 |      72 B |
|   ReadNumberTextDecimal18Max | 373.081 ns | 0.7154 ns | 1.0030 ns | 372.644 ns | 0.0224 |      96 B |
|  ReadNumberTextDecimal28Zero | 131.337 ns | 0.3463 ns | 0.5183 ns | 131.329 ns | 0.0169 |      72 B |
|   ReadNumberTextDecimal28Max | 546.280 ns | 1.1682 ns | 1.7124 ns | 545.989 ns | 0.0277 |     120 B |
|            ReadDateTimeText8 | 239.144 ns | 0.5572 ns | 0.8340 ns | 239.488 ns | 0.0172 |      72 B |
|           ReadDateTimeText14 | 304.084 ns | 0.8768 ns | 1.3123 ns | 304.399 ns | 0.0186 |      80 B |
|           ReadDateTimeText17 | 343.828 ns | 1.1230 ns | 1.6809 ns | 344.472 ns | 0.0205 |      88 B |
|               ReadText13Code |  31.936 ns | 0.1483 ns | 0.2220 ns |  31.903 ns | 0.0133 |      56 B |
|              ReadAscii13Code |  17.787 ns | 0.3568 ns | 0.5341 ns |  17.871 ns | 0.0133 |      56 B |
|             ReadText30Wide15 | 106.268 ns | 0.2572 ns | 0.3433 ns | 106.197 ns | 0.0132 |      56 B |
|          ReadUnicode30Wide15 |  14.486 ns | 0.0289 ns | 0.0385 ns |  14.481 ns | 0.0133 |      56 B |
|        ReadIntegerShort4Zero |   9.859 ns | 0.0630 ns | 0.0942 ns |   9.870 ns | 0.0057 |      24 B |
|         ReadIntegerShort4Max |  10.614 ns | 0.0438 ns | 0.0656 ns |  10.623 ns | 0.0057 |      24 B |
|             ReadInteger8Zero |  11.959 ns | 0.0406 ns | 0.0608 ns |  11.966 ns | 0.0057 |      24 B |
|              ReadInteger8Max |  14.325 ns | 0.0399 ns | 0.0560 ns |  14.316 ns | 0.0057 |      24 B |
|               ReadLong18Zero |  17.315 ns | 0.0486 ns | 0.0712 ns |  17.318 ns | 0.0057 |      24 B |
|                ReadLong18Max |  21.696 ns | 0.0462 ns | 0.0647 ns |  21.686 ns | 0.0057 |      24 B |
|             ReadDecimal8Zero |  22.598 ns | 0.0903 ns | 0.1351 ns |  22.642 ns | 0.0076 |      32 B |
|              ReadDecimal8Max |  27.519 ns | 0.0657 ns | 0.0942 ns |  27.491 ns | 0.0076 |      32 B |
|            ReadDecimal18Zero |  27.512 ns | 0.0838 ns | 0.1228 ns |  27.518 ns | 0.0076 |      32 B |
|             ReadDecimal18Max |  42.323 ns | 0.1001 ns | 0.1498 ns |  42.357 ns | 0.0076 |      32 B |
|            ReadDecimal28Zero |  33.580 ns | 0.1151 ns | 0.1723 ns |  33.578 ns | 0.0076 |      32 B |
|             ReadDecimal28Max |  82.263 ns | 0.2689 ns | 0.3941 ns |  82.320 ns | 0.0075 |      32 B |
|                ReadDateTime8 |  29.343 ns | 0.1299 ns | 0.1945 ns |  29.382 ns | 0.0057 |      24 B |
|               ReadDateTime14 |  40.121 ns | 0.1691 ns | 0.2371 ns |  40.036 ns | 0.0057 |      24 B |
|               ReadDateTime17 |  44.071 ns | 0.1913 ns | 0.2863 ns |  44.150 ns | 0.0057 |      24 B |

|                       Method |       Mean |     Error |    StdDev |     Median |  Gen 0 | Allocated |
|----------------------------- |-----------:|----------:|----------:|-----------:|-------:|----------:|
|               WriteIntBinary |   6.838 ns | 0.0186 ns | 0.0278 ns |   6.841 ns | 0.0057 |      24 B |
|                 WriteBoolean |   5.911 ns | 0.0173 ns | 0.0259 ns |   5.919 ns | 0.0057 |      24 B |
|                 WriteBytes10 |   9.574 ns | 0.0228 ns | 0.0327 ns |   9.591 ns |      - |       0 B |
|                 WriteBytes20 |   9.675 ns | 0.0257 ns | 0.0334 ns |   9.682 ns |      - |       0 B |
|      WriteSjisText20Single20 | 133.279 ns | 0.4189 ns | 0.6269 ns | 133.280 ns | 0.0112 |      48 B |
|         WriteSjisText20Wide5 |  71.828 ns | 0.2749 ns | 0.4114 ns |  71.920 ns | 0.0094 |      40 B |
|         WriteSjisText20Empty |  59.160 ns | 0.2015 ns | 0.2890 ns |  59.136 ns | 0.0132 |      56 B |
|    WriteNumberTextShort4Zero |  62.779 ns | 0.1747 ns | 0.2505 ns |  62.827 ns | 0.0209 |      88 B |
|     WriteNumberTextShort4Max |  67.673 ns | 0.1883 ns | 0.2701 ns |  67.670 ns | 0.0228 |      96 B |
|      WriteNumberTextInt8Zero |  64.641 ns | 0.2027 ns | 0.3034 ns |  64.735 ns | 0.0209 |      88 B |
|       WriteNumberTextInt8Max |  73.018 ns | 0.1542 ns | 0.2307 ns |  73.092 ns | 0.0247 |     104 B |
|    WriteNumberTextLong18Zero |  71.698 ns | 0.2757 ns | 0.4127 ns |  71.738 ns | 0.0209 |      88 B |
|     WriteNumberTextLong18Max | 104.549 ns | 0.2349 ns | 0.3515 ns | 104.664 ns | 0.0323 |     136 B |
|  WriteNumberTextDecimal8Zero |  97.028 ns | 0.2113 ns | 0.3097 ns |  96.963 ns | 0.0228 |      96 B |
|   WriteNumberTextDecimal8Max | 135.230 ns | 0.3780 ns | 0.5174 ns | 135.231 ns | 0.0284 |     120 B |
| WriteNumberTextDecimal18Zero | 102.734 ns | 0.2155 ns | 0.3225 ns | 102.774 ns | 0.0228 |      96 B |
|  WriteNumberTextDecimal18Max | 195.179 ns | 0.5505 ns | 0.7894 ns | 195.228 ns | 0.0343 |     144 B |
| WriteNumberTextDecimal28Zero | 108.861 ns | 0.3268 ns | 0.4892 ns | 108.920 ns | 0.0228 |      96 B |
|  WriteNumberTextDecimal28Max | 288.626 ns | 0.9760 ns | 1.4609 ns | 288.748 ns | 0.0415 |     176 B |
|           WriteDateTimeText8 | 264.831 ns | 0.6020 ns | 0.9011 ns | 265.173 ns | 0.0491 |     208 B |
|          WriteDateTimeText14 | 331.439 ns | 0.7034 ns | 1.0311 ns | 331.272 ns | 0.0529 |     224 B |
|          WriteDateTimeText17 | 525.116 ns | 0.9070 ns | 1.3295 ns | 524.813 ns | 0.0639 |     272 B |
|              WriteText13Code |  40.312 ns | 0.1179 ns | 0.1691 ns |  40.336 ns | 0.0095 |      40 B |
|             WriteAscii13Code |  26.085 ns | 0.0991 ns | 0.1484 ns |  26.128 ns | 0.0095 |      40 B |
|            WriteText30Wide15 |  50.745 ns | 0.5329 ns | 0.7976 ns |  50.641 ns | 0.0133 |      56 B |
|         WriteUnicode30Wide15 |   7.819 ns | 0.0568 ns | 0.0849 ns |   7.813 ns |      - |       0 B |
|       WriteIntegerShort4Zero |  12.270 ns | 0.0316 ns | 0.0463 ns |  12.261 ns | 0.0057 |      24 B |
|        WriteIntegerShort4Max |  15.156 ns | 0.0582 ns | 0.0871 ns |  15.178 ns | 0.0057 |      24 B |
|            WriteInteger8Zero |  13.886 ns | 0.0479 ns | 0.0717 ns |  13.895 ns | 0.0057 |      24 B |
|             WriteInteger8Max |  21.639 ns | 0.0542 ns | 0.0795 ns |  21.641 ns | 0.0057 |      24 B |
|              WriteLong18Zero |  19.990 ns | 0.0532 ns | 0.0745 ns |  20.020 ns | 0.0057 |      24 B |
|               WriteLong18Max |  39.034 ns | 0.0812 ns | 0.1164 ns |  39.069 ns | 0.0057 |      24 B |
|            WriteDecimal8Zero |  40.795 ns | 0.1712 ns | 0.2400 ns |  40.771 ns | 0.0171 |      72 B |
|             WriteDecimal8Max |  65.231 ns | 0.1956 ns | 0.2928 ns |  65.294 ns | 0.0170 |      72 B |
|           WriteDecimal18Zero |  50.994 ns | 0.2025 ns | 0.3030 ns |  50.953 ns | 0.0171 |      72 B |
|            WriteDecimal18Max |  98.140 ns | 0.2256 ns | 0.3163 ns |  98.253 ns | 0.0170 |      72 B |
|           WriteDecimal28Zero |  65.313 ns | 0.3273 ns | 0.4898 ns |  65.300 ns | 0.0170 |      72 B |
|            WriteDecimal28Max | 133.709 ns | 0.2878 ns | 0.4128 ns | 133.790 ns | 0.0169 |      72 B |
|               WriteDateTime8 |  32.824 ns | 0.2790 ns | 0.4090 ns |  32.595 ns | 0.0057 |      24 B |
|              WriteDateTime14 |  49.784 ns | 0.1276 ns | 0.1747 ns |  49.784 ns | 0.0057 |      24 B |
|              WriteDateTime17 |  59.034 ns | 0.6006 ns | 0.8990 ns |  58.959 ns | 0.0057 |      24 B |

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
