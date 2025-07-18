using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Contexts;
using Customer_Support_Chatbot.Models.DTOs.Payment;
using Customer_Support_Chatbot.Interfaces.Repositories;
using Customer_Support_Chatbot.Interfaces.Services;
using Customer_Support_Chatbot.Wrappers;

namespace Customer_Support_Chatbot.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<ApiResponse<Payment>> CreatePaymentAsync(PaymentDto paymentDto)
        {
            try
            {

                if (paymentDto.Amount <= 0)
                {
                    throw new ArgumentException("Payment amount must be greater than zero.");
                }   

                if (string.IsNullOrWhiteSpace(paymentDto.Currency) || string.IsNullOrWhiteSpace(paymentDto.Status))
                {
                    throw new ArgumentException("Payment currency and status cannot be null or empty.");
                }

                var addedPayment = await _paymentRepository.CreatePaymentAsync(paymentDto);

                if (addedPayment == null)
                {
                    throw new Exception("Failed to add new payment.");
                }

                return ApiResponse<Payment>.Ok("Payment created successfully.", addedPayment);
            }
            catch (ArgumentException ex)
            {
                return ApiResponse<Payment>.Fail($"Invalid payment data: {ex.Message}", 400);
            }
            catch (Exception ex)
            {
                return ApiResponse<Payment>.Fail("An error occurred while adding the payment.", 500, ex);
            }
        }

        public async Task<ApiResponse<Payment>> GetPaymentByIdAsync(Guid id)
        {
            try
            {
                var payment = await _paymentRepository.GetPaymentByIdAsync(id);
                if (payment == null)
                {
                    return ApiResponse<Payment>.Fail($"Payment with ID {id} not found.", 404);
                }
                return ApiResponse<Payment>.Ok("Payment retrieved successfully.", payment);
            }
            catch (KeyNotFoundException ex)
            {
                return ApiResponse<Payment>.Fail($"Payment with ID {id} not found.", 404, ex);
            }
            catch (Exception ex)
            {
                return ApiResponse<Payment>.Fail($"An error occurred while retrieving the payment with ID {id}.", 500, ex);
            }
        }

        public async Task<ApiResponse<IEnumerable<Payment>>> GetAllPaymentsAsync()
        {
             try
            {
                var payments = await _paymentRepository.GetAllPaymentsAsync();
                return ApiResponse<IEnumerable<Payment>>.Ok("Payments retrieved successfully.", payments);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<Payment>>.Fail("An error occurred while retrieving all payments.", 500, ex);
            }
        }
    
    }
}