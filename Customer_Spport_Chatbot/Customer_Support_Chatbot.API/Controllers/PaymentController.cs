using Microsoft.AspNetCore.Mvc;
using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Models.DTOs.Payment;
using Customer_Support_Chatbot.Services;
using Customer_Support_Chatbot.Interfaces.Services;

namespace Customer_Support_Chatbot.Controllers
{
    [ApiController]
    [Route("api/v1/payments")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentDto dto)
        {
            var result = await _paymentService.CreatePaymentAsync(dto);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(Guid id)
        {
            var result = await _paymentService.GetPaymentByIdAsync(id);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            var result = await _paymentService.GetAllPaymentsAsync();
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

    }
}
