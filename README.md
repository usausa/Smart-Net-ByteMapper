# Smart.IO.ByteMapper .NET - Fixed-length byte array mapper

| Package | Info |
|:-|:-|
| Smart.IO.ByteMapper | [![NuGet](https://img.shields.io/nuget/v/Smart.IO.ByteMapper.svg)](https://www.nuget.org/packages/Smart.IO.ByteMapper) |
| Smart.IO.ByteMapper.AspNetCore | [![NuGet](https://img.shields.io/nuget/v/Smart.IO.ByteMapper.AspNetCore.svg)](https://www.nuget.org/packages/Smart.IO.ByteMapper.AspNetCore) |

Fixed-length binary/text byte array mapping library for .NET with source generator support.

## Features

* Attribute-based mapping between C# objects and fixed-length byte arrays
* Source generator produces zero-overhead reader/writer code at compile time
* NativeAOT / trimming compatible
* ASP.NET Core integration for both MVC (input/output formatters) and Minimal API (endpoint filters)

## Getting Started

### 1. Define the model

Annotate a class with `[Map(size)]` to declare the total byte size, then use converter attributes on each property to define its offset and format.

```csharp
using Smart.IO.ByteMapper;

// Total: 59 bytes (57 data + 2-byte CRLF delimiter)
[Map(59, Delimiter = new byte[] { 0x0D, 0x0A })]
public sealed class SampleData
{
    [MapText(0, 13)]
    public string Code { get; set; } = default!;

    // CodePage 932 = Shift-JIS
    [MapText(13, 20, CodePage = 932)]
    public string Name { get; set; } = default!;

    [MapNumberText<int>(33, 6)]
    public int Qty { get; set; }

    [MapNumberText<decimal>(39, 10, Style = NumberStyles.Number)]
    public decimal Price { get; set; }

    [MapDateTimeText<DateTime>(49, 8, "yyyyMMdd")]
    public DateTime Date { get; set; }
}
```

### 2. Declare the mapper class

Create a `static partial` class and mark methods with `[ByteReader]` and `[ByteWriter]`. The source generator emits the implementation at compile time.

```csharp
using Smart.IO.ByteMapper;

internal static partial class SampleDataMappers
{
    [ByteReader]
    public static partial void Read(ReadOnlySpan<byte> source, SampleData target);

    [ByteWriter]
    public static partial void Write(Span<byte> destination, SampleData source);
}
```

### 3. Read and write

```csharp
var record = new SampleData
{
    Code = "ABC0001",
    Name = "Sample",
    Qty = 100,
    Price = 1234.56m,
    Date = new DateTime(2025, 1, 15)
};

// Write object to byte array
var buffer = new byte[59];
SampleDataMappers.Write(buffer, record);

// Read byte array back into object
var readBack = new SampleData();
SampleDataMappers.Read(buffer, readBack);
```

## Converters

| Attribute | Target types | Description |
|---|---|---|
| `[MapBinary<T>]` | `short`, `int`, `long`, `float`, `double`, `decimal`, ... | Binary numeric value; `Endian` = `Big` (default) or `Little` |
| `[MapByte]` | `byte` | Single raw byte |
| `[MapBytes]` | `byte[]` | Raw byte array with optional filler |
| `[MapText]` | `string` | Text with encoding (`CodePage`), `Trim`, and `Padding` |
| `[MapBoolean]` | `bool`, `bool?` | Single byte; configurable `TrueValue` / `FalseValue` / `NullValue` |
| `[MapNumberText<T>]` | `short`, `int`, `long`, `float`, `double`, `decimal` | Number as text with `Format`, `Padding`, `Style`, `Culture` |
| `[MapDateTimeText<T>]` | `DateTime`, `DateTimeOffset`, `DateOnly`, `TimeOnly` (and nullable variants) | Date/time as text with `Format` and `Style` |

Each converter also has a `[Map...Member]` form (e.g. `[MapTextMember]`) for describing a profile layout without re-declaring members — see [Profile-based layout switching](#profile-based-layout-switching).

## Map Options

`[Map]` accepts named options in addition to the total size.

```csharp
// 57 data bytes + 2-byte CRLF delimiter = 59 total
[Map(59, Delimiter = new byte[] { 0x0D, 0x0A })]
public sealed class SampleData { /* ... */ }
```

| Option | Default | Description |
|---|---|---|
| `Delimiter` | `null` | Delimiter bytes written at the tail of each record; occupies the last `Delimiter.Length` bytes within `Size` |
| `UseDelimiter` | `true` | When `false`, the `Delimiter` is not written even if set |
| `NullFiller` | `null` | Byte used to fill gaps between mapped fields on write; when unset, uncovered gaps are left as-is |
| `AutoFiller` | `true` | Gap auto-fill switch; when `true` **and** `NullFiller` is set, uncovered gaps are filled on write. Set `false` to disable |

Encoding, padding, filler, endianness, and boolean byte values are configured per property on the converter attributes (see the [Converters](#converters) table).

## Map Type Attributes

In addition to property-level converters, type-level attributes allow defining fixed fillers and constants within the byte layout.

```csharp
[Map(20)]
[MapFiller(10, 5)]                              // fill bytes 10–14 with 0x20
[MapConstant(15, new byte[] { 0x01, 0x00 })]   // embed constant bytes at offset 15
public sealed class MyRecord { ... }
```

## ASP.NET Core Integration

`Smart.IO.ByteMapper.AspNetCore` adds MVC input/output formatters so controllers can directly consume and produce fixed-length binary streams.

### Setup

```csharp
using Microsoft.AspNetCore.Mvc;
using Smart.IO.ByteMapper.AspNetCore;
using Smart.IO.ByteMapper.AspNetCore.Formatters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddByteMapperFormatters(o =>
{
    o.SupportedMediaTypes.Add("text/x-fixedrecord");
});

builder.Services.AddControllers();
builder.Services.AddOptions<MvcOptions>()
    .Configure<ByteMapperOutputFormatter, ByteMapperInputFormatter>(
        (mvc, output, input) =>
        {
            mvc.OutputFormatters.Insert(0, output);
            mvc.InputFormatters.Insert(0, input);
        });
```

### Mapper class for ASP.NET Core

Add `[ByteMapperEndpoint]` to the mapper class to register it with the formatter's `ByteMapperRegistry`.

```csharp
using Smart.IO.ByteMapper;
using Smart.IO.ByteMapper.AspNetCore;

[ByteMapperEndpoint]
public static partial class SampleDataMappers
{
    [ByteReader]
    public static partial void Read(ReadOnlySpan<byte> source, SampleData target);

    [ByteWriter]
    public static partial void Write(Span<byte> destination, SampleData source);
}
```

### Controller

```csharp
[Route("api/[controller]/[action]")]
public sealed class RecordController : Controller
{
    [Produces("text/x-fixedrecord")]
    [HttpGet]
    public SampleData[] GetAll() => repository.GetAll();

    [HttpPost]
    public IActionResult Post([FromBody] SampleData[] values)
    {
        // values are deserialized from the fixed-length binary request body
        return Ok(new { count = values.Length });
    }
}
```

### Minimal API

For Minimal API endpoints, `Smart.IO.ByteMapper.AspNetCore` provides route-handler filters instead of MVC formatters. Registration only needs `AddByteMapperFormatters` (the `MvcOptions` step above is MVC-only); the `[ByteMapperEndpoint]` mapper class supplies the bindings through the `ByteMapperRegistry`.

```csharp
using Smart.IO.ByteMapper.AspNetCore.Filters;

public static void MapSampleEndpoints(this WebApplication app)
{
    var group = app.MapGroup("/sample");

    // Write: handler returns SampleData[] -> binary response body
    group.MapGet("/array", () => repository.GetAll())
        .WithByteMapperArrayBody<SampleData>();

    // Write: handler returns a single SampleData -> binary response body
    group.MapGet("/single", () => repository.GetAll().First())
        .WithByteMapperBody<SampleData>();

    // Read: binary request body -> SampleData[], retrieved via GetByteMapperArrayBody<T>
    group.MapPost("/array", (HttpContext ctx)
            => Results.Ok(new { count = ctx.GetByteMapperArrayBody<SampleData>()?.Length ?? 0 }))
        .WithByteMapperArrayBody<SampleData>();

    // Read: binary request body -> single SampleData
    group.MapPost("/single", (HttpContext ctx) =>
        {
            var value = ctx.GetByteMapperBody<SampleData>();
            return value is null ? Results.BadRequest() : Results.Ok(new { value.Code });
        })
        .WithByteMapperBody<SampleData>();
}
```

| Extension | Applies to | Purpose |
|---|---|---|
| `WithByteMapperBody<T>()` | route handler | Bind a single-record binary request body; write a returned `T` as binary |
| `WithByteMapperArrayBody<T>()` | route handler | Bind a `T[]` binary request body; write a returned `T[]` as binary |
| `GetByteMapperBody<T>()` | `HttpContext` | Retrieve the parsed single record inside the handler |
| `GetByteMapperArrayBody<T>()` | `HttpContext` | Retrieve the parsed array inside the handler |

To serialize with a profile layout, use the two-type-argument overloads, e.g. `WithByteMapperArrayBody<SampleData, SampleDataCodeNameProfile>()`.

### Profile-based layout switching

When the same entity needs to be serialized with a different byte layout, define a **profile** class with `[MapProfile]` and describe each field with class-level `[Map...Member]` attributes that reference the target members by name. The profile has no members of its own, so the target entity is never duplicated.

```csharp
// Profile: only Code + Name (35 bytes), targeting the existing SampleData entity
[MapProfile(35)]
[MapTextMember(nameof(SampleData.Code), 0, 13)]
[MapTextMember(nameof(SampleData.Name), 13, 20, CodePage = 932)]
public sealed class SampleDataCodeNameProfile
{
}

// Controller action using the profile
[Produces("text/x-fixedrecord")]
[HttpGet]
[ByteMapperProfile(typeof(SampleDataCodeNameProfile))]
public SampleData[] GetCodeName() => repository.GetAll();
```

Every converter has a matching `[Map...Member]` form (`[MapTextMember]`, `[MapBinaryMember<T>]`, `[MapBooleanMember]`, …) that takes the target member name as its first argument, followed by the same parameters as the property-level attribute.

`[Map]` describes an entity's own layout (converter attributes on its properties); `[MapProfile]` describes a profile layout (`[Map...Member]` attributes on the class). Keeping them separate avoids confusing the two, and mismatched combinations are reported:

| Situation | Diagnostic |
|---|---|
| `[Map...Member]` used under `[Map]` | **SBM0015** (warning, ignored) |
| property-level converter attributes under `[MapProfile]` | **SBM0016** (warning, ignored) |
| both `[Map]` and `[MapProfile]` on one type | **SBM0017** (error) |
