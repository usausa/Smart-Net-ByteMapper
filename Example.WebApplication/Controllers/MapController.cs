namespace Example.WebApplication.Controllers
{
    using Example.WebApplication.Models;

    using Microsoft.AspNetCore.Mvc;

    using Smart.AspNetCore.Filters;

    [Route("api/[controller]")]
    public class MapController : Controller
    {
        [Produces("text/x-fixrecord")]
        [ByteMapperProfile("test")]
        [HttpGet]
        public SampleData[] Get()
        {
            return new[]
            {
                new SampleData(),
                new SampleData(),
                new SampleData()
            };
        }
    }
}
