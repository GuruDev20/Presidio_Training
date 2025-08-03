using MigrationApp.DTOs.ContactUs;
using MigrationApp.Models;

namespace MigrationApp.Interfaces.Services
{
    public interface IContactService
    {
        Task<string> AddContactUsAsync(AddContactUsDto contact);
        Task<IEnumerable<ContactU>> GetAllContactsAsync();
        Task<ContactU> GetContactByIdAsync(Guid contactId);
    }
}