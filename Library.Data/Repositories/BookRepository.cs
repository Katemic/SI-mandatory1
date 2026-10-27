using Library.Data.Context;
using Library.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryDbContext _context;

        public BookRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<Tbook?> GetByIdAsync(int id)
        {
            return await _context.Tbooks
                .AsNoTracking()
                .Include(b => b.NAuthor)
                .Include(b => b.NPublishingCompany)
                .FirstOrDefaultAsync(b => b.NBookId == id);
        }

        public async Task<Tbook> AddAsync(Tbook book)
        {
            _context.Tbooks.Add(book);
            await _context.SaveChangesAsync();

            // Return the new book with author and publisher loaded for GraphQL.
            return (await GetByIdAsync(book.NBookId))!;
        }

        public async Task<Tbook?> UpdateAsync(Tbook book)
        {
            var existingBook = await _context.Tbooks.FindAsync(book.NBookId);
            if (existingBook == null)
                return null;

            existingBook.CTitle = book.CTitle;
            existingBook.NAuthorId = book.NAuthorId;
            existingBook.NPublishingCompanyId = book.NPublishingCompanyId;
            existingBook.NPublishingYear = book.NPublishingYear;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(existingBook.NBookId);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var book = await _context.Tbooks.FindAsync(id);
            if (book == null)
                return false;

            _context.Tbooks.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
