using System.Collections.Generic;

namespace Example.WebApplication.Controllers
{
    using System;
    using System.Linq;

    using Example.WebApplication.Models;

    using Microsoft.AspNetCore.Mvc;

    using Smart.AspNetCore.Filters;

    [Route("api/[controller]/[action]")]
    public class MapController : Controller
    {
        private static SampleData[] CreateDummyData()
        {
            return new[]
            {
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
                },
            };
        }

        [Produces("text/x-fixrecord")]
        [HttpGet]
        public SampleData[] GetList()
        {
            return CreateDummyData();
        }

        [Produces("text/x-fixrecord")]
        [HttpGet]
        public SampleData[] GetEmpty()
        {
            return Array.Empty<SampleData>();
        }

        [Produces("text/x-fixrecord")]
        [HttpGet]
        public SampleData GetSingle()
        {
            return CreateDummyData().First();
        }

        [Produces("text/x-fixrecord")]
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
}
