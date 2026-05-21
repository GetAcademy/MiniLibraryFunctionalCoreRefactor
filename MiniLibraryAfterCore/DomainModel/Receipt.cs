namespace MiniLibraryAfterCore.DomainModel
{
    public record Receipt(
        string BookTitle,
        BorrowerId BorrowerId,
        DateTime DueAtUtc,
        string Message);
}
