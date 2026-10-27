using Library.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Repositories
{
    public interface IPublishingCompanyRepository
    {
        Task<Tpublishingcompany?> GetByIdAsync(int id);
        Task<List<Tpublishingcompany>> GetAllAsync();
        Task<Tpublishingcompany> AddAsync(Tpublishingcompany publisher);
        Task<Tpublishingcompany?> UpdateAsync(Tpublishingcompany publisher);
        Task<bool> DeleteAsync(int id);
    }
}
