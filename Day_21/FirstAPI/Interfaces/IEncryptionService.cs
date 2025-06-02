public interface IEncryptionService
{
    public Task<EncryptionModel> EncryptData(EncryptionModel data);

}