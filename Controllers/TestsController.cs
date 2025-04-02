using Microsoft.AspNetCore.Mvc;

namespace SEP4_User_Service.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetTest()
        {
            return Ok(new { message = "YARP forbindelsen virker, juhu!!" });
        }
    }
}
