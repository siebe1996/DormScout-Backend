using DataAccessLayer.Repositories.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.Assessments;

namespace Backend_DormScout.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class AssessmentsController : ControllerBase
    {
        private readonly IAssessmentRepository _assessmentRepository;

        public AssessmentsController(IAssessmentRepository assessmentRepository)
        {
            _assessmentRepository = assessmentRepository;
        }

        [HttpGet]
        public async Task<ActionResult<GetAssessmentModel>> GetAssessments()
        {
            var models = await _assessmentRepository.GetAssessments();
            return models == null ? NotFound() : Ok(models);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetAssessmentModel>> GetAssessment(Guid id)
        {
            var model = await _assessmentRepository.GetAssessment(id);
            return model == null ? NotFound() : Ok(model);
        }

        [HttpPost]
        public async Task<ActionResult<GetAssessmentModel>> PostAssessment(PostAssessmentModel postAssessmentModel)
        {
            var model = await _assessmentRepository.PostAssessment(postAssessmentModel);
            return CreatedAtAction("GetAssessment", new {id = model.Id}, model);
        }
    }
    
}
