using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Repositories
{
    using LibraryManagementSystem.Interfaces;
    using LibraryManagementSystem.Models;

    public class BookRepository : IBookRepository
    {
        private readonly List<Book> books = new();
        private int idCounter = 101;

        public void Add(Book book)
        {
            book.Id = idCounter++;
            books.Add(book);
        }

        public Book GetById(int id) => books.FirstOrDefault(b => b.Id == id);

        public void Update(Book book)
        {
            var existing = GetById(book.Id);
            if (existing != null)
            {
                existing.Title = book.Title;
                existing.IsBorrowed = book.IsBorrowed;
                existing.DueDate = book.DueDate;
            }
        }

        public IEnumerable<Book> GetAll() => books;
    }
}
