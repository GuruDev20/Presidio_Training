public class FileService : IFileService
{
    private readonly IFileRepository _fileRepository;
    public FileService(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }
    public async Task SaveFilesAsync(IFormFile file)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        var fileBytes = ms.ToArray();
        Console.WriteLine($"Byte array of '{file.FileName}': {BitConverter.ToString(fileBytes)}");
        var fileModel = new FileModel
        {
            FileName = file.FileName,
            FileContent = fileBytes
        };
        await _fileRepository.SaveFileAsync(fileModel);
    }
    public async Task<byte[]> GetFileAsync(string fileName)
    {
        var fileModel = await _fileRepository.GetFileAsync(fileName);
        return fileModel?.FileContent ?? Array.Empty<byte>();
    }
}