using System.Text.Json;
using MiniLibraryAfterCore.ApplicationServices;
using MiniLibraryAfterCore.DomainModel;

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
        var bookIdResult = BookId.Create(bookIdText);
        if (!bookIdResult.IsSuccess)
        {
            Console.WriteLine(bookIdResult.ErrorMessage);
            return;
        }
        var bookId = bookIdResult.Value;

        Console.Write("Borrower id: ");
        var borrowerIdText = Console.ReadLine();
        var borrowerIdResult = BorrowerId.Create(borrowerIdText);
        if (!borrowerIdResult.IsSuccess)
        {
            Console.WriteLine(borrowerIdResult.ErrorMessage);
            return;
        }
        var borrowerId = borrowerIdResult.Value!;

        var books = await LoadBooksAsync();
        var loans = await LoadLoansAsync();

        var book = books.FirstOrDefault(b => b.Id == bookId.Id);

        var borrowerAlreadyHasLoan = loans.Any(l =>
            l.BorrowerId == borrowerId.Id &&
            l.ReturnedAtUtc is null);

        var message = await LibraryService.BorrowBookAsync(book, borrowerId, borrowerAlreadyHasLoan);

        Console.WriteLine(message);
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