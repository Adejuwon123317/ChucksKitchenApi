using ChucksKitchenApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChucksKitchenApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("initialize/{orderId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> InitializePayment(int orderId)
        {
            try
            {
                var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var result = await _paymentService.InitializePaymentAsync(
                    orderId,
                    appUserId!
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("verify")]
        [Authorize]
        public async Task<IActionResult> VerifyPayment(string reference)
        {
            var verified  = await _paymentService.VerifyPaymentAsync(reference);

            if (!verified)
                return BadRequest("Payment Verification Failed.");

            return Ok("Payment Verification Successful.");
        }
    }
}
