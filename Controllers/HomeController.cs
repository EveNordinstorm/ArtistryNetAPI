using Microsoft.AspNetCore.Mvc;

namespace ArtistryNetAPI.Controllers
{
    [Route("Home/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return StatusCode(500, "An internal server error occurred.");
        }
    }
}
