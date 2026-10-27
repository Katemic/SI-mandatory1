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
    public class PublishingCompanyRepository : IPublishingCompanyRepository
    {
        private readonly LibraryDbContext _context;

        public PublishingCompanyRepository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<Tpublishingcompany?> GetByIdAsync(int id)
        {
            return await _context.Tpublishingcompanies
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.NPublishingCompanyId == id);
        }

        public async Task<List<Tpublishingcompany>> GetAllAsync()
        {
            return await _context.Tpublishingcompanies
                .AsNoTracking()
                .OrderBy(p => p.NPublishingCompanyId)
                .ToListAsync();
        }

        public async Task<Tpublishingcompany> AddAsync(Tpublishingcompany publisher)
        {
            _context.Tpublishingcompanies.Add(publisher);
            await _context.SaveChangesAsync();
            return publisher;
        }

        public async Task<Tpublishingcompany?> UpdateAsync(Tpublishingcompany publisher)
        {
            var existingPublisher = await _context.Tpublishingcompanies
                .FindAsync(publisher.NPublishingCompanyId);
            if (existingPublisher == null)
                return null;

            existingPublisher.CName = publisher.CName;

            await _context.SaveChangesAsync();
            return existingPublisher;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var publisher = await _context.Tpublishingcompanies.FindAsync(id);
            if (publisher == null)
                return false;

            _context.Tpublishingcompanies.Remove(publisher);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
