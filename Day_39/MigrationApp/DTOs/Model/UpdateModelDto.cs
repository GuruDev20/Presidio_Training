namespace MigrationApp.DTOs.Model
{
    public class UpdateModelDto
    {
        public Guid ModelId { get; set; }
        public string ModelName { get; set; } = string.Empty;
    }
}