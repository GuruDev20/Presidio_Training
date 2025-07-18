using Customer_Support_Chatbot.Contexts;
using Customer_Support_Chatbot.Interfaces.Repositories;
using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Models.DTOs.Payment;
using Microsoft.EntityFrameworkCore;

namespace Customer_Support_Chatbot.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        private new readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Payment> CreatePaymentAsync(PaymentDto paymentDto)
        {
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                Amount = paymentDto.Amount,
                Currency = paymentDto.Currency,
                Status = paymentDto.Status,
                OrderId = paymentDto.OrderId
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            return await _context.Payments
                .ToListAsync();
        }

        public async Task<Payment> GetPaymentByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Payment ID cannot be empty.", nameof(id));
            }

            return await _context.Payments
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}

