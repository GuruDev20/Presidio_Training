namespace VideoStream.API.Models
{
    public class TrainingVideo
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string BlobUrl { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    }
}