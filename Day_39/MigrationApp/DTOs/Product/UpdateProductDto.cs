namespace MigrationApp.DTOs.Product
{
    public class UpdateProductDto
    {
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; } = string.Empty;
        public string? Image { get; set; } = string.Empty;
        public double? Price { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? ColorId { get; set; }
        public Guid? ModelId { get; set; }
        public bool IsNew { get; set; }
        public DateTime? SellStartDate { get; set; }
        public DateTime? SellEndDate { get; set; }
    }
}