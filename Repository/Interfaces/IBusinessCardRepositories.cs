using Domain.Models;
using Domin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IBusinessCardRepositories : IRepository<BusinessCard>
    {
        Task<PaginatedResult<BusinessCard>> GetAllAsync(int pageNumber, int pageSize);
        Task<BusinessCard> GetByIdAsync(int id);
        Task AddAsync(BusinessCard businessCard);
        Task DeleteAsync(int id);
        Task<PaginatedResult<BusinessCard>> FilterAsync(string name, string email, string phone, string gender, DateTime? dob, int pageNumber, int pageSize);
        Task<PaginatedResult<T>> GetPaginatedResultAsync<T>(IQueryable<T> query, int pageNumber, int pageSize) where T : class;

    }
}
