using DataAccessLayer.Repositories.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Reviews;

namespace Backend_DormScout.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewsController(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        [HttpGet]
        public async Task<ActionResult<GetReviewModel>> GetReviews()
        {
            var models = await _reviewRepository.GetReviews();
            return models == null ? NotFound() : Ok(models);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetReviewModel>> GetReview(Guid id)
        {
            var model = await _reviewRepository.GetReview(id);
            return model == null ? NotFound() : Ok(model);
        }

        [HttpPost]
        public async Task<ActionResult<GetReviewModel>> PostReview(PostReviewModel postReviewModel)
        {
            var model = await _reviewRepository.PostReview(postReviewModel);
            return CreatedAtAction("GetReview", new {id = model.Id}, model);
        }
    }
}
