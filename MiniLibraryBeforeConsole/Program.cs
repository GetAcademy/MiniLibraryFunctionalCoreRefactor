using System.Text.Json;

var app = new LibraryApp();
await app.RunAsync();

public class LibraryApp
{
    private const string BooksFile = "books.json";
    private const string LoansFile = "loans.json";
    private const string ReceiptsFolder = "receipts-outbox";

    public async Task RunAsync()
    {
        await EnsureFilesExistAsync();

        Console.Write("Book id: ");
        var bookIdText = Console.ReadLine();

        Console.Write("Borrower id: ");
        var borrowerIdText = Console.ReadLine();

        if (!int.TryParse(bookIdText, out var bookId))
        {
            Console.WriteLine("Invalid book id.");
            return;
        }

        if (!int.TryParse(borrowerIdText, out var borrowerId))
        {
            Console.WriteLine("Invalid borrower id.");
            return;
        }

        var message = await BorrowBookAsync(bookId, borrowerId);

        Console.WriteLine(message);
    }

    public async Task<string> BorrowBookAsync(int bookId, int borrowerId)
    {
        var books = await LoadBooksAsync();
        var loans = await LoadLoansAsync();

        var book = books.FirstOrDefault(b => b.Id == bookId);

        if (book is null)
        {
            return "Book not found.";
        }

        if (!book.IsAvailable)
        {
            return "Book is already borrowed.";
        }

        var borrowerAlreadyHasLoan = loans.Any(l =>
            l.BorrowerId == borrowerId &&
            l.ReturnedAtUtc is null);

        if (borrowerAlreadyHasLoan)
        {
            return "Borrower already has an active loan.";
        }

        var nextLoanId = loans.Count == 0
            ? 1
            : loans.Max(l => l.Id) + 1;

        var now = DateTime.UtcNow;

        var loan = new Loan
        {
            Id = nextLoanId,
            BookId = book.Id,
            BorrowerId = borrowerId,
            BorrowedAtUtc = now,
            DueAtUtc = now.AddDays(14)
        };

        loans.Add(loan);

        book.IsAvailable = false;

        await SaveLoansAsync(loans);
        await SaveBooksAsync(books);

        var receipt = new Receipt(
            BookTitle: book.Title,
            BorrowerId: borrowerId,
            DueAtUtc: loan.DueAtUtc,
            Message: $"You borrowed '{book.Title}'.");

        await WriteReceiptAsync(receipt);

        return "Book borrowed.";
    }

    private async Task<List<Book>> LoadBooksAsync()
    {
        var json = await File.ReadAllTextAsync(BooksFile);

        return JsonSerializer.Deserialize<List<Book>>(json)
            ?? new List<Book>();
    }

    private async Task<List<Loan>> LoadLoansAsync()
    {
        var json = await File.ReadAllTextAsync(LoansFile);

        return JsonSerializer.Deserialize<List<Loan>>(json)
            ?? new List<Loan>();
    }

    private async Task SaveBooksAsync(List<Book> books)
    {
        var json = JsonSerializer.Serialize(
            books,
            new JsonSerializerOptions { WriteIndented = true });

        await File.WriteAllTextAsync(BooksFile, json);
    }

    private async Task SaveLoansAsync(List<Loan> loans)
    {
        var json = JsonSerializer.Serialize(
            loans,
            new JsonSerializerOptions { WriteIndented = true });

        await File.WriteAllTextAsync(LoansFile, json);
    }

    private async Task WriteReceiptAsync(Receipt receipt)
    {
        Directory.CreateDirectory(ReceiptsFolder);

        var fileName =
            $"{DateTime.UtcNow:yyyyMMdd-HHmmssfff}-{Guid.NewGuid()}.json";

        var path = Path.Combine(ReceiptsFolder, fileName);

        var json = JsonSerializer.Serialize(
            receipt,
            new JsonSerializerOptions { WriteIndented = true });

        await File.WriteAllTextAsync(path, json);
    }

    private async Task EnsureFilesExistAsync()
    {
        if (!File.Exists(BooksFile))
        {
            var books = new List<Book>
            {
                new()
                {
                    Id = 1,
                    Title = "Clean Code",
                    IsAvailable = true
                }
            };

            await SaveBooksAsync(books);
        }

        if (!File.Exists(LoansFile))
        {
            await SaveLoansAsync(new List<Loan>());
        }
    }
}

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public bool IsAvailable { get; set; }
}

public class Loan
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int BorrowerId { get; set; }
    public DateTime BorrowedAtUtc { get; set; }
    public DateTime DueAtUtc { get; set; }
    public DateTime? ReturnedAtUtc { get; set; }
}

public record Receipt(
    string BookTitle,
    int BorrowerId,
    DateTime DueAtUtc,
    string Message);
