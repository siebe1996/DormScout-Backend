using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Models.Tests;
using Models.Users;

namespace Backend_DormScout.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class TestController : ControllerBase
    {
        [EnableCors("AllowAnyOrigins")]
        [AllowAnonymous]
        [HttpGet("Hello-world")]
        public async Task<ActionResult<GetHelloModel>> HelloWorld()
        {
            try
            {
                var responseModel = new GetHelloModel
                {
                    Hello = "hello world",
                };

                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
