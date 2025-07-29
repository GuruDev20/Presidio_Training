using Razorpay.Api;
using System.Collections.Generic;

namespace Customer_Support_Chatbot.Interfaces.Services
{
    public interface IRazorpayService
    {
        public Order CreateOrder(int amountInPaise);
    }
}