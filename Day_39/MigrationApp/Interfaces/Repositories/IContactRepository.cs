using MigrationApp.DTOs.ContactUs;
using MigrationApp.Models;

namespace MigrationApp.Interfaces.Repositories
{
    public interface IContactRepository
    {
        Task<IEnumerable<ContactU>> GetAllContactsAsync();
        Task<ContactU> GetContactByIdAsync(Guid contactId);
        Task<string> AddContactAsync(AddContactUsDto addContactUsDto);
    }
}