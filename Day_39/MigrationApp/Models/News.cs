namespace MigrationApp.Models
{
    public class News
    {
        public Guid NewsId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<bool> Status { get; set; }
    }
}