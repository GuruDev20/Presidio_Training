namespace MigrationApp.Models
{
    public class Model
    {
        public Guid ModelId { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public virtual ICollection<Product>? Products { get; set; }
    }
}