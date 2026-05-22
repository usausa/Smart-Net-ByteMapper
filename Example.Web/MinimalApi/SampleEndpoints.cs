namespace Example.Web.MinimalApi;

using Example.Web.Models;

using Smart.IO.ByteMapper.AspNetCore.Filters;

public static class SampleEndpoints
{
    private static SampleData[] CreateDummyData() =>
    [
        new()
        {
            Code = "1111111111111",
            Name = "あああああ",
            Qty = 123,
            Price = 100m,
            Date = DateTime.Today
        },
        new()
        {
            Code = "2222222222222",
            Name = "いいいいいい",
            Qty = 888888,
            Price = 9999999.99m,
            Date = DateTime.Today.AddDays(-1)
        },
        new()
        {
            Code = "XXXXXXXXXXXXX",
            Name = "Sample data",
            Qty = 10000,
            Price = 49.50m,
            Date = default
        }
    ];

    public static void MapSampleEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/minimal/sample");

        // GET: return array of SampleData as binary
        group.MapGet("/array", CreateDummyData)
            .WithByteMapperArrayBody<SampleData>()
            .WithName("MinimalGetArray");

        // GET: return single SampleData as binary
        group.MapGet("/single", () => CreateDummyData().First())
            .WithByteMapperBody<SampleData>()
            .WithName("MinimalGetSingle");

        // GET: return empty array
        group.MapGet("/empty", Array.Empty<SampleData>)
            .WithByteMapperArrayBody<SampleData>()
            .WithName("MinimalGetEmpty");

        // GET: return short format (SampleDataShort, 35 bytes per record)
        group.MapGet("/short", () => CreateDummyData()
                .Select(d => new SampleDataShort { Code = d.Code, Name = d.Name })
                .ToArray())
            .WithByteMapperArrayBody<SampleDataShort>()
            .WithName("MinimalGetShort");

        // POST: receive array of SampleData from binary body (body is parsed by the ByteMapper filter)
        group.MapPost("/array", (HttpContext ctx)
                => Results.Ok(new { count = ctx.GetByteMapperArrayBody<SampleData>()?.Length ?? 0 }))
            .WithByteMapperArrayBody<SampleData>()
            .WithName("MinimalPostArray");

        // POST: receive single SampleData from binary body
        group.MapPost("/single", (HttpContext ctx) =>
            {
                var value = ctx.GetByteMapperBody<SampleData>();
                return value is null ? Results.BadRequest() : Results.Ok(new { code = value.Code, name = value.Name });
            })
            .WithByteMapperBody<SampleData>()
            .WithName("MinimalPostSingle");

        // ---- Profile: SampleDataCodeNameProfile (35 bytes per record, code + name fields only) ----

        // GET: return array of SampleData serialised using the "code-name" profile
        group.MapGet("/profile/code-name", CreateDummyData)
            .WithByteMapperArrayBody<SampleData, SampleDataCodeNameProfile>()
            .WithName("MinimalGetProfileCodeName");

        // POST: receive SampleData records using the "code-name" profile
        group.MapPost("/profile/code-name", (HttpContext ctx)
                => Results.Ok(new { count = ctx.GetByteMapperArrayBody<SampleData>()?.Length ?? 0 }))
            .WithByteMapperArrayBody<SampleData, SampleDataCodeNameProfile>()
            .WithName("MinimalPostProfileCodeName");
    }
}
