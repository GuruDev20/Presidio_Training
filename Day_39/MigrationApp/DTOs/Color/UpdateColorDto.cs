namespace MigrationApp.DTOs.Color
{
    public class UpdateColorDto
    {
        public Guid ColorId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}