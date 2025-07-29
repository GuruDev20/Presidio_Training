using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Models.DTOs.Payment;

namespace Customer_Support_Chatbot.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        public Task<Payment> CreatePaymentAsync(PaymentDto paymentDto);
        public Task<IEnumerable<Payment>> GetAllPaymentsAsync();
        public Task<Payment> GetPaymentByIdAsync(Guid id);
    }
}
