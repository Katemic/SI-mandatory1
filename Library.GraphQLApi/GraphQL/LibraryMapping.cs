using Library.Data.Models;
using static Library.GraphQLApi.GraphQL.LibraryTypes;

namespace Library.GraphQLApi.GraphQL
{
    internal static class LibraryMapping
    {
        public static Author ToGraphQl(Tauthor author) =>
            new(author.NAuthorId, author.CName, author.CSurname);

        public static Publisher ToGraphQl(Tpublishingcompany publisher) =>
            new(publisher.NPublishingCompanyId, publisher.CName);

        public static Book ToGraphQl(Tbook book) =>
            new(
                book.NBookId,
                book.CTitle,
                book.NAuthorId,
                book.NPublishingCompanyId,
                book.NPublishingYear,
                ToGraphQl(book.NAuthor),
                ToGraphQl(book.NPublishingCompany));
    }
}
