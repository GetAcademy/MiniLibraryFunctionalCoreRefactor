namespace MiniLibraryAfterCore.DomainModel
{
    public class Book
    {
        public BookId Id { get; set; }
        public string Title { get; set; } = "";
        public bool IsAvailable { get; set; }
    }
}
