namespace MiniLibraryAfterCore.DomainModel
{
    public record BorrowBookResult(Loan Loan, Receipt Receipt, Book Book)
    {
    }
}
