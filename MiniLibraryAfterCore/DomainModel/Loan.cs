namespace MiniLibraryAfterCore.DomainModel
{
    public class Loan
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int BorrowerId { get; set; }
        public DateTime BorrowedAtUtc { get; set; }
        public DateTime DueAtUtc { get; set; }
        public DateTime? ReturnedAtUtc { get; set; }
    }
}
