namespace Example.Web.MinimalApi;

using System.Buffers;

using Example.Web.Models;

using Smart.IO.ByteMapper.AspNetCore;
using Smart.IO.ByteMapper.AspNetCore.Filters;

public static class SampleEndpoints
{
    private static SampleData[] CreateDummyData() =>
    [
        new SampleData
        {
            Code = "1111111111111",
            Name = "あああああ",
            Qty = 123,
            Price = 100m,
            Date = DateTime.Today
        },
        new SampleData
        {
            Code = "2222222222222",
            Name = "いいいいいい",
            Qty = 888888,
            Price = 9999999.99m,
            Date = DateTime.Today.AddDays(-1)
        },
        new SampleData
        {
            Code = "XXXXXXXXXXXXX",
            Name = "Sample data",
            Qty = 10000,
            Price = 49.50m,
            Date = default
        }
    ];

    public static void MapSampleEndpoints(this WebApplication app, ByteMapperRegistry registry)
    {
        var group = app.MapGroup("/minimal/sample");

        // GET: return array of SampleData as binary
        group.MapGet("/array", () => CreateDummyData())
            .WithByteMapperArrayBody<SampleData>()
            .WithName("MinimalGetArray")
            .WithOpenApi();

        // GET: return single SampleData as binary
        group.MapGet("/single", () => CreateDummyData().First())
            .WithByteMapperBody<SampleData>()
            .WithName("MinimalGetSingle")
            .WithOpenApi();

        // GET: return empty array
        group.MapGet("/empty", () => Array.Empty<SampleData>())
            .WithByteMapperArrayBody<SampleData>()
            .WithName("MinimalGetEmpty")
            .WithOpenApi();

        // GET: return short format (SampleDataShort, 35 bytes per record)
        group.MapGet("/short", () => CreateDummyData()
                .Select(d => new SampleDataShort { Code = d.Code, Name = d.Name })
                .ToArray())
            .WithByteMapperArrayBody<SampleDataShort>()
            .WithName("MinimalGetShort")
            .WithOpenApi();

        // POST: receive array of SampleData from binary body
        // ByteMapperRegistry is injected from DI; body is read manually to avoid JSON binding.
        group.MapPost("/array", async (HttpContext ctx) =>
            {
                var binding = registry.GetBinding<SampleData>()
                    ?? throw new InvalidOperationException("Binding for SampleData not found.");
                var items = await ReadArrayAsync(ctx.Request.Body, binding, ctx.RequestAborted);
                return Results.Ok(new { count = items.Length });
            })
            .WithName("MinimalPostArray");

        // POST: receive single SampleData from binary body
        group.MapPost("/single", async (HttpContext ctx) =>
            {
                var binding = registry.GetBinding<SampleData>()
                    ?? throw new InvalidOperationException("Binding for SampleData not found.");
                var buffer = ArrayPool<byte>.Shared.Rent(binding.Size);
                try
                {
                    var read = await ctx.Request.Body.ReadAsync(buffer.AsMemory(0, binding.Size), ctx.RequestAborted);
                    if (read < binding.Size)
                    {
                        return Results.BadRequest("Incomplete body.");
                    }

                    var value = binding.Create();
                    binding.Read(buffer.AsSpan(0, binding.Size), value);
                    return Results.Ok(new { code = value.Code, name = value.Name });
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            })
            .WithName("MinimalPostSingle");

        // ---- Profile: "code-name" (35 bytes per record, code + name fields only) ----

        // GET: return array of SampleData serialised using the "code-name" profile
        group.MapGet("/profile/code-name", () =>
            {
                var binding = registry.GetArrayBinding<SampleData>("code-name")
                    ?? throw new InvalidOperationException("Binding 'code-name' not registered.");

                var data = CreateDummyData();
                var buffer = new byte[binding.ElementSize * data.Length];
                for (var i = 0; i < data.Length; i++)
                {
                    binding.WriteElement(data[i], buffer.AsSpan(i * binding.ElementSize, binding.ElementSize));
                }

                return Results.Bytes(buffer, "application/octet-stream");
            })
            .WithName("MinimalGetProfileCodeName")
            .WithOpenApi();

        // POST: receive SampleData records using the "code-name" profile
        group.MapPost("/profile/code-name", async (HttpContext ctx) =>
            {
                var binding = registry.GetBinding<SampleData>("code-name")
                    ?? throw new InvalidOperationException("Binding 'code-name' not registered.");

                var items = await ReadArrayAsync(ctx.Request.Body, binding, ctx.RequestAborted);
                return Results.Ok(new { count = items.Length });
            })
            .WithName("MinimalPostProfileCodeName");
    }

    private static async Task<SampleData[]> ReadArrayAsync(
        Stream body,
        ByteMapperBinding<SampleData> binding,
        CancellationToken ct)
    {
        var size = binding.Size;
        var buffer = ArrayPool<byte>.Shared.Rent(size);
        var items = new List<SampleData>();
        try
        {
            int read;
            while ((read = await body.ReadAsync(buffer.AsMemory(0, size), ct)) == size)
            {
                var item = binding.Create();
                binding.Read(buffer.AsSpan(0, size), item);
                items.Add(item);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        return [.. items];
    }
}

