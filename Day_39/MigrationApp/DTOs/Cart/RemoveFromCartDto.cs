namespace MigrationApp.DTOs.Cart
{
    public class RemoveFromCartDto
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
    }
}