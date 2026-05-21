namespace MiniLibraryAfterCore.DomainModel
{
    public class Loan
    {
        public Guid Id { get; set; }
        public BookId BookId { get; set; }
        public BorrowerId BorrowerId { get; set; }
        public DateTime BorrowedAt { get; set; }
        public DateTime DueAtUtc { get; set; }
        public DateTime? ReturnedAtUtc { get; set; }

        public Loan()
        {
            Id = Guid.NewGuid();
        }
    }
}
