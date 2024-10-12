using Domain.Models;
using Domain.Models.BusinessCardDto;
using Domin.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IBusinessCardService
    {
        Task<PaginatedResult<BusinessCard>> GetAllAsync(int pageNumber, int pageSize);
        Task<BusinessCard> GetByIdAsync(int id);
        Task<dynamic> GetBusinessCardsById(int id);
        Task CreateBusinessCard(BusinessCardDto businessCardDto, IFormFile file);
        Task DeleteAsync(int id);
        Task<PaginatedResult<BusinessCard>> FilterBusinessCardsAsync(string name, string email, string phone, string gender, DateTime? dob, int pageNumber, int pageSize);
        Task<byte[]> ExportBusinessCardsAsync(string format, int Id);
        Task<(List<string> errors, BusinessCard validData)> ValidateBusinessCardImport(IFormFile file);


    }
}
