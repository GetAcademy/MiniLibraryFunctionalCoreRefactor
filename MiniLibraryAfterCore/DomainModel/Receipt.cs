namespace MiniLibraryAfterCore.DomainModel
{
    public record Receipt(
        string BookTitle,
        int BorrowerId,
        DateTime DueAtUtc,
        string Message);
}
