using Domain.Models;
using Domin.Context;
using Domin.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Common;
using Repository.Interfaces;
using Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class BusinessCardRepositories : Repository<BusinessCard>, IBusinessCardRepositories
    {

        private BusinessCardDBContext _context;
        public BusinessCardRepositories(BusinessCardDBContext context) : base(context)
        {
            _context = context;
        }


        public async Task<PaginatedResult<BusinessCard>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.BusinessCards.Include(b => b.Attachments);
            return await GetPaginatedResultAsync(query, pageNumber, pageSize);
        }

        public async Task<BusinessCard> GetByIdAsync(int id)
        {
            return await _context.BusinessCards
                .Include(b => b.Attachments)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddAsync(BusinessCard businessCard)
        {
            await _context.BusinessCards.AddAsync(businessCard);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var businessCard = await GetByIdAsync(id);
            if (businessCard == null) return;

            _context.BusinessCards.Remove(businessCard);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedResult<BusinessCard>> FilterAsync(string name, string email, string phone, string gender, DateTime? dob, int pageNumber, int pageSize)
        {
            var query = _context.BusinessCards
                .Include(b => b.Attachments)
                .AsQueryable();

            query = ApplyFilters(query, name, email, phone, gender, dob);

            return await GetPaginatedResultAsync(query, pageNumber, pageSize);
        }

        private IQueryable<BusinessCard> ApplyFilters(IQueryable<BusinessCard> query, string? name, string? email, string? phone, string? gender, DateTime? dob)
        {
            if (!string.IsNullOrEmpty(name))
                query = query.Where(b => b.Name.ToLower().Trim().Contains(name.ToLower().Trim()));

            if (!string.IsNullOrEmpty(email))
                query = query.Where(b => b.Email.ToLower().Trim().Contains(email.ToLower().Trim()));

            if (!string.IsNullOrEmpty(phone))
                query = query.Where(b => b.Phone.Trim().Contains(phone.Trim()));

            if (!string.IsNullOrEmpty(gender))
                query = query.Where(b => b.Gender == gender);

            if (dob.HasValue)
                query = query.Where(b => b.DateOfBirth.Date == dob.Value.Date);

            return query;
        }


        public async Task<PaginatedResult<T>> GetPaginatedResultAsync<T>(IQueryable<T> query, int pageNumber, int pageSize) where T : class
        {

            var totalCount = await query.CountAsync();
            var paginatedData = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<T>
            {
                Data = paginatedData,
                TotalCount = totalCount
            };
        }
    }
}

