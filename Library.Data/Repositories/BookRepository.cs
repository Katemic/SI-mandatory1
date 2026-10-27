using Library.Data.Context;
using Library.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryDbContext _dbContext;

        public BookRepository(LibraryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Tbook?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken)
        {
            return _dbContext.Tbooks
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    book => book.NBookId == id,
                    cancellationToken);
        }

        public Task<bool> AuthorExistsAsync(
            int id,
            CancellationToken cancellationToken)
        {
            return _dbContext.Tauthors
                .AnyAsync(author => author.NAuthorId == id, cancellationToken);
        }

        public Task<bool> PublishingCompanyExistsAsync(
            int id,
            CancellationToken cancellationToken)
        {
            return _dbContext.Tpublishingcompanies
                .AnyAsync(
                    company => company.NPublishingCompanyId == id,
                    cancellationToken);
        }

        public async Task<Tbook> CreateAsync(
            Tbook book,
            CancellationToken cancellationToken)
        {
            _dbContext.Tbooks.Add(book);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return book;
        }
    }
}
