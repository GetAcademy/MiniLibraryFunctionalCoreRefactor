using MiniLibraryAfterCore.DomainModel;

namespace MiniLibraryAfterCore.ApplicationServices
{
    public class LibraryService
    {
        public static Result<BorrowBookResult> BorrowBook(Book? book, BorrowerId borrowerId, bool borrowerAlreadyHasLoan)
        {
            if (book is null)
            {
                return Result<BorrowBookResult>.Fail("Book not found.");
            }

            if (!book.IsAvailable)
            {
                return Result<BorrowBookResult>.Fail("Book is already borrowed.");
            }

            if (borrowerAlreadyHasLoan)
            {
                return Result<BorrowBookResult>.Fail("Borrower already has an active loan.");
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
    }
}
