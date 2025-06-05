using System.Security.Cryptography;
using System.Text;
using HRDocumentAPI.Interfaces;
using HRDocumentAPI.Models;

namespace HRDocumentAPI.Services
{
    public class EncryptionService : IEncryptionService
    {
        public async Task<EncryptionModel> EncryptData(EncryptionModel data)
        {
            HMACSHA256 hMACSHA256 = new HMACSHA256();
            data.EncryptedData = hMACSHA256.ComputeHash(Encoding.UTF8.GetBytes(data.Data));
            return data;
        }
    }
}
