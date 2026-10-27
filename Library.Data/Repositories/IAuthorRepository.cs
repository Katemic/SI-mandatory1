using Library.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Repositories
{
    public interface IAuthorRepository
    {
        Task<Tauthor?> GetByIdAsync(int id);
        Task<List<Tauthor>> GetAllAsync();
        Task<Tauthor> AddAsync(Tauthor author);
        Task<Tauthor?> UpdateAsync(Tauthor author);
        Task<bool> DeleteAsync(int id);
    }
}
