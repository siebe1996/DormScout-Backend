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
    public class PossibleDatesController : ControllerBase
    {
        private readonly IPossibleDateRepository _possibleDateRepository;

        public PossibleDatesController(IPossibleDateRepository possibleDateRepository)
        {
            _possibleDateRepository = possibleDateRepository;
        }
    }
}
