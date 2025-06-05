public interface IFileRepository
{
    public Task SaveFileAsync(FileModel fileModel);
    public Task<FileModel> GetFileAsync(string fileName);
}