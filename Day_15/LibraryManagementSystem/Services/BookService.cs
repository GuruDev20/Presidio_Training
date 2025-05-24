using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services
{
    using LibraryManagementSystem.Interfaces;
    using LibraryManagementSystem.Models;

    public class BookService
    {
        private readonly IBookRepository _repository;
        private readonly ILogger _logger;
        public BookService(IBookRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public void AddBook(Book book)
        {
            _repository.Add(book);
            _logger.Log($"Book added: {book.Title}");
        }
    }
}
