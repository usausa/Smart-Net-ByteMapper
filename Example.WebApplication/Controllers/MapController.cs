namespace Example.WebApplication.Controllers
{
    using Example.WebApplication.Models;

    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    public class MapController : Controller
    {
        [Produces("text/x-fixrecord")]
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
