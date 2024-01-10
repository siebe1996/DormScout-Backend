using DataAccessLayer.Repositories;
using DataAccessLayer.Repositories.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Assessments;
using Models.Authentication;
using Stripe;

namespace Backend_DormScout.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class PaymentIntentController : ControllerBase
    {
        private readonly IPaymentIntentRepository _paymentIntentRepository;

        public PaymentIntentController(IPaymentIntentRepository paymentIntentRepository)
        {
            _paymentIntentRepository = paymentIntentRepository;
        }

        [HttpPost("create")]
        public async Task<ActionResult<ClientSecretModel>> Create(PaymentIntentCreateRequestModel request)
        {
            var model = await _paymentIntentRepository.Create(request);
            return model;
        }
    }
}
