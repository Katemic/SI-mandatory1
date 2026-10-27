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
    public class AuthorRepository : IAuthorRepository
    {
        private readonly LibraryDbContext _context;

        public AuthorRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<Tauthor?> GetByIdAsync(int id)
        {
            return await _context.Tauthors
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.NAuthorId == id);
        }

        public async Task<List<Tauthor>> GetAllAsync()
        {
            return await _context.Tauthors
                .AsNoTracking()
                .OrderBy(a => a.NAuthorId)
                .ToListAsync();
        }

        public async Task<Tauthor> AddAsync(Tauthor author)
        {
            _context.Tauthors.Add(author);
            await _context.SaveChangesAsync();
            return author;
        }

        public async Task<Tauthor?> UpdateAsync(Tauthor author)
        {
            var existingAuthor = await _context.Tauthors.FindAsync(author.NAuthorId);
            if (existingAuthor == null)
                return null;

            existingAuthor.CName = author.CName;
            existingAuthor.CSurname = author.CSurname;

            await _context.SaveChangesAsync();
            return existingAuthor;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var author = await _context.Tauthors.FindAsync(id);
            if (author == null)
                return false;

            _context.Tauthors.Remove(author);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
