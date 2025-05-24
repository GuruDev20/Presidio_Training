using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Utilities;

class Program
{
    static void Main(string[] args)
    {
        IBookRepository bookRepository = new BookRepository();
        IMemberRepository memberRepository = new MemberRepository();
        ILogger logger = new Logger();
        IPenaltyCalculator penaltyCalculator = new StandardPenaltyCalculator();

        var bookService = new BookService(bookRepository, logger);
        var memberService = new MemberService(memberRepository, logger);
        var borrowService = new BorrowService(bookRepository, memberRepository, penaltyCalculator, logger);

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Library Management System\n");
            Console.WriteLine("1. Add Book");
            Console.WriteLine("2. Register Member");
            Console.WriteLine("3. Borrow Book");
            Console.WriteLine("4. Return Book");
            Console.WriteLine("5. Exit");
            Console.Write("Select option: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.Write("Enter book title: ");
                    var title = Console.ReadLine();
                    bookService.AddBook(new Book { Title = title });
                    break;
                case "2":
                    Console.Write("Enter member name: ");
                    var name = Console.ReadLine();
                    memberService.RegisterMember(new Member { Name = name });
                    break;
                case "3":
                    Console.Write("Enter book ID: ");
                    int bookId = int.Parse(Console.ReadLine());
                    Console.Write("Enter member ID: ");
                    int memberId = int.Parse(Console.ReadLine());
                    borrowService.BorrowBook(bookId, memberId);
                    break;
                case "4":
                    Console.Write("Enter book ID: ");
                    int returnBookId = int.Parse(Console.ReadLine());
                    borrowService.ReturnBook(returnBookId);
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
    }
}