using HRDocumentAPI.Models;

namespace HRDocumentAPI.Interfaces
{
    public interface IEncryptionService
    {
        public Task<EncryptionModel> EncryptData(EncryptionModel data);
    }
}