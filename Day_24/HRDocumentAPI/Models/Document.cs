namespace HRDocumentAPI.Models
{
    public class Document
    {
        public Guid Id { get; set; }
        public string Filname { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public Guid UploadedById { get; set; }
        public User? UploadedBy { get; set; }
    }
}