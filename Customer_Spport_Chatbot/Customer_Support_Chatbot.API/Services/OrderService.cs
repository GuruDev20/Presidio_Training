using Customer_Support_Chatbot.Models;
using Customer_Support_Chatbot.Contexts;
using Customer_Support_Chatbot.Models.DTOs.Order;
using Customer_Support_Chatbot.Interfaces.Repositories;
using Customer_Support_Chatbot.Interfaces.Services;
using Customer_Support_Chatbot.Wrappers;

namespace Customer_Support_Chatbot.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IRazorpayService _razorpayService;
        public OrderService(IOrderRepository orderRepository, IRazorpayService razorpayService)
        {
            _orderRepository = orderRepository;
            _razorpayService = razorpayService;
        }

        public async Task<ApiResponse<Order>> CreateOrderAsync(OrderDto orderDto)
        {
            try
            {
                Console.WriteLine("Validating orderDto fields...");
                if (string.IsNullOrWhiteSpace(orderDto.CustomerName) || string.IsNullOrWhiteSpace(orderDto.Email) || string.IsNullOrWhiteSpace(orderDto.ContactNumber))
                {
                    Console.WriteLine("Validation failed: Missing required fields.");
                    throw new ArgumentException("Order customer name, email, and contact number cannot be null or empty.");
                }

                var razorpayOrder = _razorpayService.CreateOrder(orderDto.Amount * 100);
                if (razorpayOrder == null)
                {
                    Console.WriteLine("Failed to create Razorpay order.");
                    throw new Exception("Failed to create Razorpay order.");
                }

                var addedOrder = await _orderRepository.CreateOrderAsync(orderDto, razorpayOrder["id"].ToString());
                if (addedOrder == null)
                {
                    Console.WriteLine("Failed to add new order to repository.");
                    throw new Exception("Failed to add new order.");
                }

                Console.WriteLine("Order added successfully.");
                return ApiResponse<Order>.Ok("Order created successfully.", addedOrder);

            }
            catch (ArgumentException ex)
            {
                return ApiResponse<Order>.Fail($"Invalid order data: {ex.Message}", 400);
            }
            catch (Exception ex)
            {
                return ApiResponse<Order>.Fail("An error occurred while adding the order.", 500, ex);
            }
        }

        public async Task<ApiResponse<Order>> GetOrderByIdAsync(Guid id)
        {
            try
            {
                var order = await _orderRepository.GetOrderByIdAsync(id);
                if (order == null)
                {
                    Console.WriteLine($"Order with ID {id} not found.");
                    return ApiResponse<Order>.Fail($"Order with ID {id} not found.", 404);
                }
                return ApiResponse<Order>.Ok("Order retrieved successfully.", order);
            }
            catch (KeyNotFoundException ex)
            {
                return ApiResponse<Order>.Fail($"Order with ID {id} not found.", 404, ex);
            }
            catch (Exception ex)
            {
                return ApiResponse<Order>.Fail($"An error occurred while retrieving the order with ID {id}.", 500, ex);
            }
        }

        public async Task<ApiResponse<IEnumerable<Order>>> GetAllOrdersAsync()
        {
             try
            {
                var orders = await _orderRepository.GetAllOrdersAsync();
                return ApiResponse<IEnumerable<Order>>.Ok("Orders retrieved successfully.", orders);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<Order>>.Fail("An error occurred while retrieving all orders.", 500, ex);
            }
        }
    
    }
}