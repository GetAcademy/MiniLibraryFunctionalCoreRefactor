namespace MiniLibraryAfterCore.DomainModel
{
    public record BookId
    {
        public int Id { get; }

        private BookId(int id)
        {
            Id = id;
        }

        public static Result<BookId> Create(string id)
        {
            return !int.TryParse(id, out var bookId) 
                ? Result<BookId>.Fail("Invalid book id.") 
                : Result<BookId>.Success(new BookId(bookId));
        }
    }
}
