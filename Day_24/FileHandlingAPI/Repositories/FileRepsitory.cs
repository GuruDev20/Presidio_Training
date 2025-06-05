public class FileRepository : IFileRepository
{
    private static readonly Dictionary<string, FileModel> _fileStorage = new();
    public Task SaveFileAsync(FileModel file)
    {
        if (file == null || string.IsNullOrEmpty(file.FileName) || file.FileContent == null)
        {
            throw new ArgumentException("Invalid file model.");
        }

        _fileStorage[file.FileName] = file;
        return Task.CompletedTask;
    }
    public Task<FileModel> GetFileAsync(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentException("File name cannot be null or empty.");
        }

        _fileStorage.TryGetValue(fileName, out var fileModel);
        return Task.FromResult(fileModel);
    }
}