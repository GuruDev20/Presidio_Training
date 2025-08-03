namespace MigrationApp.DTOs.News
{
    public class UpdateNewsDto
    {
        public Guid NewsId { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? Image { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? Status { get; set; }
    }
}