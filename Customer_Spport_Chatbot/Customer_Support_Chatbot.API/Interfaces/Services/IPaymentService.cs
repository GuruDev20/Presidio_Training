
using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Models.DTOs.Payment;
using Customer_Support_Chatbot.Wrappers;

namespace Customer_Support_Chatbot.Interfaces.Services
{
    public interface IPaymentService
    {
        public Task<ApiResponse<Payment>> CreatePaymentAsync(PaymentDto paymentDto);
        public Task<ApiResponse<IEnumerable<Payment>>> GetAllPaymentsAsync();
        public Task<ApiResponse<Payment>> GetPaymentByIdAsync(Guid id);
    }
}