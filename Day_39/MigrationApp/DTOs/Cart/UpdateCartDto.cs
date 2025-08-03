namespace MigrationApp.DTOs.Cart
{
    public class UpdateCartDto
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}