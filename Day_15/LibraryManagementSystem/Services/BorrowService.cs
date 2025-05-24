using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Services
{
    using LibraryManagementSystem.Interfaces;
    using LibraryManagementSystem.Models;

    public class BorrowService
    {
        private readonly IBookRepository _bookRepo;
        private readonly IMemberRepository _memberRepo;
        private readonly IPenaltyCalculator _penaltyCalculator;
        private readonly ILogger _logger;

        public BorrowService(IBookRepository bookRepo, IMemberRepository memberRepo, IPenaltyCalculator penaltyCalculator, ILogger logger)
        {
            _bookRepo = bookRepo;
            _memberRepo = memberRepo;
            _penaltyCalculator = penaltyCalculator;
            _logger = logger;
        }

        public void BorrowBook(int bookId, int memberId)
        {
            var book = _bookRepo.GetById(bookId);
            var member = _memberRepo.GetById(memberId);
            if (book == null || member == null || book.IsBorrowed)
            {
                _logger.Log("Borrowing failed.");
                return;
            }

            book.IsBorrowed = true;
            book.DueDate = DateTime.Now.AddDays(14);
            _bookRepo.Update(book);
            _logger.Log($"Book borrowed: {book.Title} by {member.Name}");
            _logger.Log($"{member.Name}, you borrowed '{book.Title}' due on {book.DueDate:dd/MM/yyyy}");
        }

        public void ReturnBook(int bookId)
        {
            var book = _bookRepo.GetById(bookId);
            if (book == null || !book.IsBorrowed)
            {
                _logger.Log("Return failed.");
                return;
            }

            var penalty = _penaltyCalculator.CalculatePenalty(book.DueDate.Value, DateTime.Now);
            book.IsBorrowed = false;
            book.DueDate = null;
            _bookRepo.Update(book);
            _logger.Log($"Book returned: {book.Title}. Penalty: ${penalty:F2}");
        }
    }
}
