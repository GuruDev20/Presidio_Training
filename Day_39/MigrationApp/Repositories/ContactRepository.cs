using Microsoft.EntityFrameworkCore;
using MigrationApp.Contexts;
using MigrationApp.DTOs.ContactUs;
using MigrationApp.Interfaces.Repositories;
using MigrationApp.Models;

namespace MigrationApp.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly AppDbContext _context;
        public ContactRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> AddContactAsync(AddContactUsDto addContactUsDto)
        {
            if (addContactUsDto == null)
            {
                throw new ArgumentNullException(nameof(addContactUsDto));
            }
            var contact = new ContactU
            {
                Id = Guid.NewGuid(),
                Name = addContactUsDto.Name,
                Email = addContactUsDto.Email,
                Content = addContactUsDto.Message,
            };
            _context.ContactUs.Add(contact);
            await _context.SaveChangesAsync();
            return contact.Id.ToString();
        }

        public async Task<IEnumerable<ContactU>> GetAllContactsAsync()
        {
            var contacts = await _context.ContactUs.ToListAsync();
            return contacts;
        }

        public async Task<ContactU> GetContactByIdAsync(Guid contactId)
        {
            if (contactId == Guid.Empty)
            {
                throw new ArgumentException("Contact ID cannot be empty.", nameof(contactId));
            }
            var contact = await _context.ContactUs.FindAsync(contactId);
            if (contact == null)
            {
                throw new KeyNotFoundException("Contact not found.");
            }
            return contact;
        }
    }
}