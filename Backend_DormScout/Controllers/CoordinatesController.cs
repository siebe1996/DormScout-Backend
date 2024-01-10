using DataAccessLayer.Repositories.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend_DormScout.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class CoordinatesController : ControllerBase
    {
        private readonly ICoordinateRepository _coordinateRepository;

        public CoordinatesController(ICoordinateRepository coordinateRepository)
        {
            _coordinateRepository = coordinateRepository;
        }
    }
}
