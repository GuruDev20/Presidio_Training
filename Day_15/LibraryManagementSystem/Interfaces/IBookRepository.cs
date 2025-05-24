using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Interfaces
{
    using LibraryManagementSystem.Models;
    public interface IBookRepository
    {
        void Add(Book book);
        Book GetById(int id);
        void Update(Book book);
        IEnumerable<Book> GetAll();
    }
}
}
