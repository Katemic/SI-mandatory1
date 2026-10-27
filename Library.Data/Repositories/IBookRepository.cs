using Library.Data.Models;

namespace Library.Data.Repositories
{
    public interface IBookRepository
    {
        Task<Tbook?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> AuthorExistsAsync(int id, CancellationToken cancellationToken);
        Task<bool> PublishingCompanyExistsAsync(int id, CancellationToken cancellationToken);
        Task<Tbook> CreateAsync(Tbook book, CancellationToken cancellationToken);
    }
}
