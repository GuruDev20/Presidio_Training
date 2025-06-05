public interface IFileService
{
    public Task SaveFilesAsync(IFormFile file);
    public Task<byte[]> GetFileAsync(string fileName);
}