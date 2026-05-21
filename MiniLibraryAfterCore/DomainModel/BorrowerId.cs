namespace MiniLibraryAfterCore.DomainModel
{
    public class BorrowerId
    {
        public int Id { get; }

        private BorrowerId(int id)
        {
            Id = id;
        }

        public static Result<BorrowerId> Create(string id)
        {
            return !int.TryParse(id, out var borrowerId)
                ? Result<BorrowerId>.Fail("Invalid borrower id.")
                : Result<BorrowerId>.Success(new BorrowerId(borrowerId));
        }
    }

}
