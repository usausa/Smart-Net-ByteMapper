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
        public SampleData[] List()
        {
            return CreateDummyData();
        }

        [Produces("text/x-fixrecord")]
        [HttpGet]
        public SampleData[] Empty()
        {
            return Array.Empty<SampleData>();
        }

        [Produces("text/x-fixrecord")]
        [HttpGet]
        public SampleData Single()
        {
            return CreateDummyData().First();
        }

        [Produces("text/x-fixrecord")]
        [ByteMapperProfile("short")]
        [HttpGet]
        public SampleData[] Profile()
        {
            return CreateDummyData();
        }

        // TODO input
    }
}
