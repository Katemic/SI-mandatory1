using Library.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Data.Repositories
{
    public interface IBookRepository
    {
        Task<Tbook?> GetByIdAsync(int id);
        Task<Tbook> AddAsync(Tbook book);
        Task<Tbook?> UpdateAsync(Tbook book);
        Task<bool> DeleteAsync(int id);
    }
}
