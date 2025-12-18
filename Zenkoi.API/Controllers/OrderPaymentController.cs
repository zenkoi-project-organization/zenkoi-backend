using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderPaymentController : BaseAPIController
    {
        private readonly IPaymentService _paymentService;

        public OrderPaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create-payment/{orderId:int}")]
        public async Task<IActionResult> CreatePaymentFromOrder(int orderId, [FromQuery] string method = "VnPay")
        {
            try
            {
                var result = await _paymentService.CreatePaymentLinkAsync(orderId, method);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Error creating payment: {ex.Message}");
            }
        }  
    }
}

