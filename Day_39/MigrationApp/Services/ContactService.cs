using MigrationApp.DTOs.ContactUs;
using MigrationApp.Interfaces.Repositories;
using MigrationApp.Interfaces.Services;
using MigrationApp.Models;

namespace MigrationApp.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        public ContactService(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<string> AddContactUsAsync(AddContactUsDto contact)
        {
            if (contact == null)
            {
                throw new ArgumentNullException(nameof(contact));
            }
            return await _contactRepository.AddContactAsync(contact);
        }

        public async Task<IEnumerable<ContactU>> GetAllContactsAsync()
        {
            return await _contactRepository.GetAllContactsAsync();
        }

        public async Task<ContactU> GetContactByIdAsync(Guid contactId)
        {
            if (contactId == Guid.Empty)
            {
                throw new ArgumentException("Contact ID cannot be empty.", nameof(contactId));
            }
            return await _contactRepository.GetContactByIdAsync(contactId);
        }
    }
}