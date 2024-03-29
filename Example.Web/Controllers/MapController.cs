namespace Example.Web.Controllers;

using Example.Web.Models;

using Microsoft.AspNetCore.Mvc;

using Smart.AspNetCore.Filters;

// ReSharper disable StringLiteralTypo
#pragma warning disable IDE0060
[Route("api/[controller]/[action]")]
public sealed class MapController : Controller
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

    private static SampleData[] CreateDummyData()
    {
        return
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
                Date = null
            }
        ];
    }

    [Produces("text/x-fixedrecord")]
    [HttpGet]
    public SampleData[] GetLarge()
    {
        return LargeValues;
    }

    [Produces("text/x-fixedrecord")]
    [HttpGet]
    public SampleData[] GetArray()
    {
        return CreateDummyData();
    }

    [Produces("text/x-fixedrecord")]
    [HttpGet]
    public IEnumerable<SampleData> GetList()
    {
        return new List<SampleData>(CreateDummyData());
    }

    [Produces("text/x-fixedrecord")]
    [HttpGet]
    public SampleData[] GetEmpty()
    {
        return [];
    }

    [Produces("text/x-fixedrecord")]
    [HttpGet]
    public SampleData GetSingle()
    {
        return CreateDummyData().First();
    }

    [Produces("text/x-fixedrecord")]
    [ByteMapperProfile("short")]
    [HttpGet]
    public SampleData[] GetProfile()
    {
        return CreateDummyData();
    }

    [HttpPost]
    public IActionResult PostArray([FromBody] SampleData[] values)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok();
    }

    [HttpPost]
    public IActionResult PostEnumerable([FromBody] IEnumerable<SampleData> values)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok();
    }

    [HttpPost]
    public IActionResult PostSingle([FromBody] SampleData value)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok();
    }

    [HttpPost]
    [ByteMapperProfile("short")]
    public IActionResult PostProfile([FromBody] SampleData[] values)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok();
    }
}
