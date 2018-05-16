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
* 1,000,000 op/s object to byte array

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
|                ReadIntBinary |   6.486 ns | 0.1426 ns | 0.2045 ns |   6.457 ns | 0.0057 |      24 B |
|                  ReadBoolean |   4.711 ns | 0.0367 ns | 0.0538 ns |   4.714 ns | 0.0057 |      24 B |
|                  ReadBytes10 |  10.088 ns | 0.0438 ns | 0.0614 ns |  10.085 ns | 0.0095 |      40 B |
|                  ReadBytes20 |  10.625 ns | 0.0559 ns | 0.0837 ns |  10.609 ns | 0.0114 |      48 B |
|       ReadSjisText20Single20 | 116.123 ns | 0.5843 ns | 0.8380 ns | 116.073 ns | 0.0170 |      72 B |
|          ReadSjisText20Wide5 |  81.214 ns | 0.4883 ns | 0.7158 ns |  81.059 ns | 0.0094 |      40 B |
|          ReadSjisText20Empty |  38.380 ns | 0.3776 ns | 0.5415 ns |  38.344 ns |      - |       0 B |
|     ReadNumberTextShort4Zero |  96.071 ns | 0.4926 ns | 0.7064 ns |  95.964 ns | 0.0132 |      56 B |
|      ReadNumberTextShort4Max | 118.049 ns | 0.5308 ns | 0.7613 ns | 117.790 ns | 0.0150 |      64 B |
|       ReadNumberTextInt8Zero |  95.535 ns | 0.3591 ns | 0.5375 ns |  95.444 ns | 0.0132 |      56 B |
|        ReadNumberTextInt8Max | 138.164 ns | 0.5192 ns | 0.7279 ns | 138.005 ns | 0.0172 |      72 B |
|     ReadNumberTextLong18Zero | 100.706 ns | 0.4506 ns | 0.6605 ns | 100.629 ns | 0.0132 |      56 B |
|      ReadNumberTextLong18Max | 207.498 ns | 0.7450 ns | 1.0919 ns | 207.388 ns | 0.0207 |      88 B |
|   ReadNumberTextDecimal8Zero | 114.147 ns | 0.6877 ns | 1.0081 ns | 113.986 ns | 0.0151 |      64 B |
|    ReadNumberTextDecimal8Max | 229.644 ns | 1.0534 ns | 1.5767 ns | 229.285 ns | 0.0188 |      80 B |
|  ReadNumberTextDecimal18Zero | 127.223 ns | 0.3967 ns | 0.5815 ns | 127.116 ns | 0.0169 |      72 B |
|   ReadNumberTextDecimal18Max | 430.695 ns | 1.1177 ns | 1.6383 ns | 430.641 ns | 0.0224 |      96 B |
|  ReadNumberTextDecimal28Zero | 137.147 ns | 0.6260 ns | 0.9369 ns | 137.085 ns | 0.0169 |      72 B |
|   ReadNumberTextDecimal28Max | 638.932 ns | 2.0216 ns | 2.8993 ns | 638.484 ns | 0.0277 |     120 B |
|            ReadDateTimeText8 | 266.650 ns | 1.1171 ns | 1.6720 ns | 266.521 ns | 0.0172 |      72 B |
|           ReadDateTimeText14 | 381.978 ns | 1.5786 ns | 2.3139 ns | 381.550 ns | 0.0186 |      80 B |
|           ReadDateTimeText17 | 474.765 ns | 1.0581 ns | 1.5175 ns | 474.828 ns | 0.0205 |      88 B |
|               ReadText13Code |  35.153 ns | 0.1462 ns | 0.2050 ns |  35.129 ns | 0.0133 |      56 B |
|              ReadAscii13Code |  18.216 ns | 0.0600 ns | 0.0861 ns |  18.201 ns | 0.0133 |      56 B |
|             ReadText30Wide15 | 130.345 ns | 4.9002 ns | 7.3343 ns | 127.726 ns | 0.0131 |      56 B |
|          ReadUnicode30Wide15 |  15.406 ns | 0.1219 ns | 0.1787 ns |  15.365 ns | 0.0133 |      56 B |
|        ReadIntegerShort4Zero |  11.287 ns | 0.0448 ns | 0.0628 ns |  11.275 ns | 0.0057 |      24 B |
|         ReadIntegerShort4Max |  13.868 ns | 0.0652 ns | 0.0936 ns |  13.864 ns | 0.0057 |      24 B |
|             ReadInteger8Zero |  12.574 ns | 0.0528 ns | 0.0774 ns |  12.568 ns | 0.0057 |      24 B |
|              ReadInteger8Max |  18.484 ns | 0.0713 ns | 0.1067 ns |  18.479 ns | 0.0057 |      24 B |
|               ReadLong18Zero |  17.364 ns | 0.0573 ns | 0.0784 ns |  17.362 ns | 0.0057 |      24 B |
|                ReadLong18Max |  29.209 ns | 0.4450 ns | 0.6522 ns |  29.420 ns | 0.0057 |      24 B |
|             ReadDecimal8Zero |  24.895 ns | 0.1589 ns | 0.2329 ns |  24.882 ns | 0.0076 |      32 B |
|              ReadDecimal8Max |  31.893 ns | 0.1662 ns | 0.2384 ns |  31.881 ns | 0.0076 |      32 B |
|            ReadDecimal18Zero |  31.754 ns | 0.4161 ns | 0.6100 ns |  32.022 ns | 0.0076 |      32 B |
|             ReadDecimal18Max |  51.911 ns | 0.2840 ns | 0.4250 ns |  51.756 ns | 0.0076 |      32 B |
|            ReadDecimal28Zero |  37.954 ns | 0.1859 ns | 0.2782 ns |  37.856 ns | 0.0076 |      32 B |
|             ReadDecimal28Max |  96.747 ns | 0.3547 ns | 0.4972 ns |  96.728 ns | 0.0075 |      32 B |
|                ReadDateTime8 |  41.086 ns | 0.1776 ns | 0.2490 ns |  41.026 ns | 0.0057 |      24 B |
|               ReadDateTime14 |  53.845 ns | 0.1424 ns | 0.2042 ns |  53.790 ns | 0.0057 |      24 B |
|               ReadDateTime17 |  63.442 ns | 0.3080 ns | 0.4514 ns |  63.329 ns | 0.0056 |      24 B |

|                       Method |       Mean |     Error |    StdDev |     Median |  Gen 0 | Allocated |
|----------------------------- |-----------:|----------:|----------:|-----------:|-------:|----------:|
|               WriteIntBinary |   6.213 ns | 0.0428 ns | 0.0614 ns |   6.204 ns | 0.0057 |      24 B |
|                 WriteBoolean |   5.490 ns | 0.0346 ns | 0.0508 ns |   5.480 ns | 0.0057 |      24 B |
|                 WriteBytes10 |  10.002 ns | 0.0277 ns | 0.0407 ns |   9.999 ns |      - |       0 B |
|                 WriteBytes20 |  10.001 ns | 0.0264 ns | 0.0370 ns |   9.996 ns |      - |       0 B |
|      WriteSjisText20Single20 | 152.808 ns | 1.9677 ns | 2.9451 ns | 153.031 ns | 0.0112 |      48 B |
|         WriteSjisText20Wide5 |  81.990 ns | 0.4932 ns | 0.7383 ns |  81.839 ns | 0.0094 |      40 B |
|         WriteSjisText20Empty |  68.241 ns | 0.3471 ns | 0.5087 ns |  68.156 ns | 0.0132 |      56 B |
|    WriteNumberTextShort4Zero |  89.880 ns | 0.3439 ns | 0.5040 ns |  89.763 ns | 0.0209 |      88 B |
|     WriteNumberTextShort4Max |  98.373 ns | 0.6762 ns | 0.9912 ns |  98.069 ns | 0.0228 |      96 B |
|      WriteNumberTextInt8Zero |  86.942 ns | 0.3106 ns | 0.4454 ns |  86.929 ns | 0.0209 |      88 B |
|       WriteNumberTextInt8Max | 103.770 ns | 0.3092 ns | 0.4435 ns | 103.749 ns | 0.0247 |     104 B |
|    WriteNumberTextLong18Zero |  92.471 ns | 0.4052 ns | 0.5546 ns |  92.383 ns | 0.0209 |      88 B |
|     WriteNumberTextLong18Max | 150.199 ns | 0.4960 ns | 0.7271 ns | 150.214 ns | 0.0322 |     136 B |
|  WriteNumberTextDecimal8Zero | 115.099 ns | 0.5693 ns | 0.8344 ns | 115.123 ns | 0.0228 |      96 B |
|   WriteNumberTextDecimal8Max | 150.123 ns | 0.7247 ns | 1.0847 ns | 149.745 ns | 0.0284 |     120 B |
| WriteNumberTextDecimal18Zero | 121.022 ns | 0.4119 ns | 0.5908 ns | 121.049 ns | 0.0226 |      96 B |
|  WriteNumberTextDecimal18Max | 210.966 ns | 0.8295 ns | 1.1896 ns | 210.818 ns | 0.0343 |     144 B |
| WriteNumberTextDecimal28Zero | 126.932 ns | 0.6901 ns | 0.9898 ns | 126.773 ns | 0.0226 |      96 B |
|  WriteNumberTextDecimal28Max | 272.553 ns | 1.2145 ns | 1.8178 ns | 272.036 ns | 0.0415 |     176 B |
|           WriteDateTimeText8 | 337.203 ns | 5.9221 ns | 8.4932 ns | 332.034 ns | 0.0491 |     208 B |
|          WriteDateTimeText14 | 414.927 ns | 2.9669 ns | 4.3488 ns | 415.294 ns | 0.0529 |     224 B |
|          WriteDateTimeText17 | 582.998 ns | 4.0923 ns | 5.6016 ns | 581.444 ns | 0.0639 |     272 B |
|              WriteText13Code |  43.765 ns | 0.1710 ns | 0.2397 ns |  43.695 ns | 0.0095 |      40 B |
|             WriteAscii13Code |  27.182 ns | 0.1562 ns | 0.2290 ns |  27.082 ns | 0.0095 |      40 B |
|            WriteText30Wide15 |  58.515 ns | 0.4739 ns | 0.7093 ns |  58.472 ns | 0.0132 |      56 B |
|         WriteUnicode30Wide15 |   7.921 ns | 0.0474 ns | 0.0649 ns |   7.913 ns |      - |       0 B |
|       WriteIntegerShort4Zero |  13.164 ns | 0.0637 ns | 0.0893 ns |  13.150 ns | 0.0057 |      24 B |
|        WriteIntegerShort4Max |  16.313 ns | 0.0814 ns | 0.1193 ns |  16.294 ns | 0.0057 |      24 B |
|            WriteInteger8Zero |  14.332 ns | 0.0809 ns | 0.1161 ns |  14.323 ns | 0.0057 |      24 B |
|             WriteInteger8Max |  21.926 ns | 0.0721 ns | 0.1057 ns |  21.894 ns | 0.0057 |      24 B |
|              WriteLong18Zero |  20.693 ns | 0.0792 ns | 0.1161 ns |  20.678 ns | 0.0057 |      24 B |
|               WriteLong18Max |  38.914 ns | 0.1673 ns | 0.2399 ns |  38.947 ns | 0.0057 |      24 B |
|            WriteDecimal8Zero |  43.770 ns | 0.1509 ns | 0.2115 ns |  43.738 ns | 0.0171 |      72 B |
|             WriteDecimal8Max |  70.361 ns | 0.2492 ns | 0.3653 ns |  70.256 ns | 0.0170 |      72 B |
|           WriteDecimal18Zero |  57.822 ns | 0.3497 ns | 0.5234 ns |  57.741 ns | 0.0171 |      72 B |
|            WriteDecimal18Max | 108.380 ns | 0.4705 ns | 0.6747 ns | 108.093 ns | 0.0170 |      72 B |
|           WriteDecimal28Zero |  72.077 ns | 0.7710 ns | 1.1540 ns |  71.973 ns | 0.0170 |      72 B |
|            WriteDecimal28Max | 141.834 ns | 0.6468 ns | 0.9481 ns | 141.652 ns | 0.0169 |      72 B |
|               WriteDateTime8 |  55.509 ns | 0.1592 ns | 0.2334 ns |  55.492 ns | 0.0057 |      24 B |
|              WriteDateTime14 |  85.063 ns | 0.3249 ns | 0.4447 ns |  85.036 ns | 0.0056 |      24 B |
|              WriteDateTime17 |  97.219 ns | 1.2174 ns | 1.7845 ns |  97.495 ns | 0.0056 |      24 B |

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

* DateTimeConverter performance improvement.
* Add DateTime(specific?) and Guid(?) support to BinaryCoverter.
* Add Nullable support to BinaryCoverter.
* Add Double and Float support to NumberTextCoverter.
* Add DoubleConverter and FloatConverter.
* Performance improvements using Span.
