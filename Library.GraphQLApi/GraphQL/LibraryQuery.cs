using HotChocolate;
using Library.Data.Context;
using Library.Data.Models;
using Library.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using static Library.GraphQLApi.GraphQL.LibraryTypes;

namespace Library.GraphQLApi.GraphQL
{
    public class LibraryQuery
    {
        [GraphQLName("book")]
        public async Task<Book?> GetBookAsync(int id, [Service] IBookRepository books)
        {
            var book = await books.GetByIdAsync(id);
            return book == null ? null : LibraryMapping.ToGraphQl(book);
        }

        [GraphQLName("authors")]
        public async Task<List<Author>> GetAuthorsAsync([Service] IAuthorRepository authors)
        {
            var result = await authors.GetAllAsync();
            return result.Select(LibraryMapping.ToGraphQl).ToList();
        }

        [GraphQLName("publishers")]
        public async Task<List<Publisher>> GetPublishersAsync(
            [Service] IPublishingCompanyRepository publishers)
        {
            var result = await publishers.GetAllAsync();
            return result.Select(LibraryMapping.ToGraphQl).ToList();
        }
    }
}

