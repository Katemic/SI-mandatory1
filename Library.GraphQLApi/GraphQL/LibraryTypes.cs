namespace Library.GraphQLApi.GraphQL
{
    public class LibraryTypes
    {
        public sealed record Author(int Id, string Name, string Surname);

        public sealed record Publisher(int Id, string Name);

        public sealed record Book(
            int Id,
            string Title,
            int AuthorId,
            int PublisherId,
            int PublishingYear,
            Author Author,
            Publisher Publisher);
    }
}
