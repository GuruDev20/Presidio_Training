namespace MigrationApp.DTOs.Order
{
    public class AddOrderDto
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string PaymentType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
    }
}