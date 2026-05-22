namespace Example.Web.Controllers;

using System.Buffers;

using Example.Web.Models;

using Microsoft.AspNetCore.Mvc;

using Smart.IO.ByteMapper.AspNetCore;
using Smart.IO.ByteMapper.AspNetCore.Filters;

// ReSharper disable StringLiteralTypo
[Route("api/[controller]/[action]")]
public sealed class MapController(ByteMapperRegistry registry) : Controller
{
    private static readonly SampleData[] LargeValues = new SampleData[1000];

    static MapController()
    {
        for (var i = 0; i < LargeValues.Length; i++)
        {
            LargeValues[i] = new SampleData
            {
                Code = "1111111111111",
                Name = "あああああ",
                Qty = 123,
                Price = 100m,
                Date = DateTime.Today
            };
        }
    }

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

    [Produces("text/x-fixedrecord")]
    [HttpGet]
    public SampleData[] GetLarge() => LargeValues;

    [Produces("text/x-fixedrecord")]
    [HttpGet]
    public SampleData[] GetArray() => CreateDummyData();

    [Produces("text/x-fixedrecord")]
    [HttpGet]
    public IEnumerable<SampleData> GetList() => new List<SampleData>(CreateDummyData());

    [Produces("text/x-fixedrecord")]
    [HttpGet]
    public SampleData[] GetEmpty() => [];

    [Produces("text/x-fixedrecord")]
    [HttpGet]
    public SampleData GetSingle() => CreateDummyData().First();

    [HttpPost]
    public IActionResult PostArray([FromBody] SampleData[] values)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(new { count = values.Length });
    }

    [HttpPost]
    public IActionResult PostEnumerable([FromBody] IEnumerable<SampleData> values)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(new { count = values.Count() });
    }

    [HttpPost]
    public IActionResult PostSingle([FromBody] SampleData value)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(new { code = value.Code, name = value.Name });
    }

    // ---- Profile: "code-name" (35 bytes per record, code + name fields only) ----

    /// <summary>
    /// Returns SampleData serialised using the "code-name" profile (code and name only).
    /// </summary>
    [HttpGet]
    public IActionResult GetProfileCodeName()
    {
        var binding = registry.GetArrayBinding<SampleData>("code-name");
        if (binding is null)
        {
            return StatusCode(500, "Binding 'code-name' not registered.");
        }

        var data = CreateDummyData();
        var buffer = new byte[binding.ElementSize * data.Length];
        for (var i = 0; i < data.Length; i++)
        {
            binding.WriteElement(data[i], buffer.AsSpan(i * binding.ElementSize, binding.ElementSize));
        }

        return File(buffer, "application/octet-stream");
    }

    /// <summary>
    /// Receives SampleData records serialised using the "code-name" profile (code and name only).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> PostProfileCodeName()
    {
        var binding = registry.GetBinding<SampleData>("code-name");
        if (binding is null)
        {
            return StatusCode(500, "Binding 'code-name' not registered.");
        }

        var size = binding.Size;
        var buffer = ArrayPool<byte>.Shared.Rent(size);
        var items = new List<SampleData>();
        try
        {
            int read;
            while ((read = await Request.Body.ReadAsync(buffer.AsMemory(0, size), HttpContext.RequestAborted)) == size)
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

        return Ok(new { count = items.Count });
    }
}
