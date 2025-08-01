using Razorpay.Api;
using System.Collections.Generic;
using Customer_Support_Chatbot.Interfaces.Services;

namespace Customer_Support_Chatbot.Services
{
    public class RazorpayService : IRazorpayService
    {
        private const string key = "rzp_test_gFjcpJyALvQmt1";
        private const string secret = "ahgsC2mDxQ8VMwNYMLY7QXmC";

        public Order CreateOrder(int amountInPaise)
        {
            RazorpayClient client = new RazorpayClient(key, secret);

            Dictionary<string, object> options = new Dictionary<string, object>
        {
            { "amount", amountInPaise }, 
            { "currency", "INR" },
            { "receipt", "receipt#1" },
            { "payment_capture", 1 }
        };

            Order order = client.Order.Create(options);
            return order;
        }
    }
}