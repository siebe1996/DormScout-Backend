using DataAccessLayer.Repositories.interfaces;
using Globals.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Places;
using Models.Authentication;
using Stripe;
using System.Text;
using Microsoft.Extensions.DependencyInjection;


namespace Backend_DormScout.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class PlacesController : ControllerBase
    {
        private readonly IPlaceRepository _placeRepository;

        public PlacesController(IPlaceRepository placeRepository)
        {
            _placeRepository = placeRepository;
        }

        [HttpGet("Reviewer")]
        public async Task<ActionResult<GetPlaceModel>> GetPlacesReviewer()
        {
            var models = await _placeRepository.GetPlacesReviewer();
            return models == null ? NotFound() : Ok(models);
        }

        [HttpGet("NotYours")]
        public async Task<ActionResult<GetPlaceModel>> GetPlacesNotYours()
        {
            var models = await _placeRepository.GetPlacesNotYours();
            return models == null ? NotFound() : Ok(models);
        }

        [HttpGet("Yours")]
        public async Task<ActionResult<GetPlaceModel>> GetPlacesYours()
        {
            var models = await _placeRepository.GetPlacesYours();
            return models == null ? NotFound() : Ok(models);
        }

        [HttpGet("Nearby")]
        public async Task<ActionResult<GetPlaceModel>> GetPlacesNearby([FromQuery] double lat, [FromQuery] double lon)
        {
            var models = await _placeRepository.GetPlacesNearby(lat, lon);
            return models == null ? NotFound() : Ok(models);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetPlaceModel>> GetPlace(Guid id)
        {
            var model = await _placeRepository.GetPlace(id);
            return model == null ? NotFound() : Ok(model);
        }

        [HttpPost]
        public async Task<ActionResult<GetPlaceModel>> PostPlace(PostPlaceModel postPlaceModel)
        {
            GetPlaceModel model = await _placeRepository.PostPlace(postPlaceModel);
            return CreatedAtAction("GetPlace", new { id = model.Id }, model);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GetPlaceModel>> PutPlace([FromRoute] Guid id, [FromBody] PutPlaceModel putPlaceModel)
        {
            GetPlaceModel model = await _placeRepository.PutPlace(id, putPlaceModel);
            return model != null ? Ok(model) : NotFound();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<GetPlaceModel>> PatchPlace([FromRoute] Guid id, [FromBody] PatchPlaceModel patchPlaceModel)
        {
            GetPlaceModel model = await _placeRepository.PatchPlace(id, patchPlaceModel);
            return model != null ? Ok(model) : NotFound();
        }

        [HttpPost("create-payment-intent")]
        public async Task<ActionResult<ClientSecretModel>> Create(PaymentIntentCreateRequestModel request)
        {
            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
            {
                Amount = CalculateOrderAmount(request.Items),
                Currency = "eur",
                // In the latest version of the API, specifying the `automatic_payment_methods` parameter is optional because Stripe enables its functionality by default.
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            });

            return new ClientSecretModel { ClientSecret = paymentIntent.ClientSecret };
        }

        private int CalculateOrderAmount(Item[] items)
        {
            // Replace this constant with a calculation of the order's amount
            // Calculate the order total on the server to prevent
            // people from directly manipulating the amount on the client
            return 1400;
        }
    }
}
