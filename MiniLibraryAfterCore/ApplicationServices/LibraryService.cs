using MiniLibraryAfterCore.DomainModel;

namespace MiniLibraryAfterCore.ApplicationServices
{
    public class LibraryService
    {
        public static Result<BorrowBookResult> BorrowBook(Book? book, BorrowerId borrowerId, bool borrowerAlreadyHasLoan, DateTime now)
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

            var loan = new Loan
            {
                BookId = book.Id,
                BorrowerId = borrowerId,
                BorrowedAt = now,
                DueAtUtc = now.AddDays(14)
            };
            
            //book.IsAvailable = false;
            book = book with { IsAvailable = true };


            var receipt = new Receipt(
                BookTitle: book.Title,
                BorrowerId: borrowerId,
                DueAtUtc: loan.DueAtUtc,
                Message: $"You borrowed '{book.Title}'.");

            return Result<BorrowBookResult>.Success(new BorrowBookResult(loan, receipt, book));
        }
    }
}
