using HotChocolate;
using Library.Data.Models;
using Library.Data.Repositories;
using static Library.GraphQLApi.GraphQL.LibraryTypes;

namespace Library.GraphQLApi.GraphQL
{
    public class LibraryMutation
    {
        [GraphQLName("createBook")]
        public async Task<Book> CreateBookAsync(
        string title, int authorId, int publisherId, int publishingYear,
        [Service] IBookRepository books)
        {
            var created = await books.AddAsync(new Tbook
            {
                CTitle = title,
                NAuthorId = authorId,
                NPublishingCompanyId = publisherId,
                NPublishingYear = publishingYear
            });

            return LibraryMapping.ToGraphQl(created);
        }

        [GraphQLName("updateBook")]
        public async Task<Book?> UpdateBookAsync(
            int id, string title, int authorId, int publisherId, int publishingYear,
            [Service] IBookRepository books)
        {
            var updated = await books.UpdateAsync(new Tbook
            {
                NBookId = id,
                CTitle = title,
                NAuthorId = authorId,
                NPublishingCompanyId = publisherId,
                NPublishingYear = publishingYear
            });

            return updated == null ? null : LibraryMapping.ToGraphQl(updated);
        }

        [GraphQLName("deleteBook")]
        public Task<bool> DeleteBookAsync(int id, [Service] IBookRepository books)
        {
            return books.DeleteAsync(id);
        }

        [GraphQLName("createAuthor")]
        public async Task<Author> CreateAuthorAsync(
            string name, string surname, [Service] IAuthorRepository authors)
        {
            var created = await authors.AddAsync(new Tauthor
            {
                CName = name,
                CSurname = surname
            });

            return LibraryMapping.ToGraphQl(created);
        }

        [GraphQLName("updateAuthor")]
        public async Task<Author?> UpdateAuthorAsync(
            int id, string name, string surname, [Service] IAuthorRepository authors)
        {
            var updated = await authors.UpdateAsync(new Tauthor
            {
                NAuthorId = id,
                CName = name,
                CSurname = surname
            });

            return updated == null ? null : LibraryMapping.ToGraphQl(updated);
        }

        [GraphQLName("deleteAuthor")]
        public Task<bool> DeleteAuthorAsync(int id, [Service] IAuthorRepository authors)
        {
            return authors.DeleteAsync(id);
        }

        [GraphQLName("createPublisher")]
        public async Task<Publisher> CreatePublisherAsync(
            string name, [Service] IPublishingCompanyRepository publishers)
        {
            var created = await publishers.AddAsync(new Tpublishingcompany
            {
                CName = name
            });

            return LibraryMapping.ToGraphQl(created);
        }

        [GraphQLName("updatePublisher")]
        public async Task<Publisher?> UpdatePublisherAsync(
            int id, string name, [Service] IPublishingCompanyRepository publishers)
        {
            var updated = await publishers.UpdateAsync(new Tpublishingcompany
            {
                NPublishingCompanyId = id,
                CName = name
            });

            return updated == null ? null : LibraryMapping.ToGraphQl(updated);
        }

        [GraphQLName("deletePublisher")]
        public Task<bool> DeletePublisherAsync(
            int id, [Service] IPublishingCompanyRepository publishers)
        {
            return publishers.DeleteAsync(id);
        }
    }
}
