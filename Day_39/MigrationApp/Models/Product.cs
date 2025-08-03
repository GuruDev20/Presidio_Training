namespace MigrationApp.Models
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public Nullable<double> Price { get; set; }
        public Nullable<Guid> CategoryId { get; set; }
        public Nullable<Guid> ColorId { get; set; }
        // public Nullable<Guid> ModelId { get; set; }
        public Nullable<DateTime> SellStartDate { get; set; }
        public Nullable<DateTime> SellEndDate { get; set; }
        public Nullable<bool> IsNew { get; set; }
        public virtual Category? Category { get; set; }
        public virtual Color? Color { get; set; }
        // public virtual Model? Model { get; set; }
    }
}