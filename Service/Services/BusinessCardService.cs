using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Vml.Office;
using Domain.Models;
using Domain.Models.BusinessCardDto;
using Domin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Repository.Interfaces;
using Service.Interfaces;
using Service.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Service.Services
{
    public class BusinessCardService : IBusinessCardService
    {
        private IRepositoryUnitOfWork _repositoryUnitOfWork;
        private ITPServiceUnitOfWork _tPServiceUnitOfWork;


        public BusinessCardService(IRepositoryUnitOfWork repositoryUnitOfWork, ITPServiceUnitOfWork tPServiceUnitOfWork)
        {
            _repositoryUnitOfWork = repositoryUnitOfWork;
            _tPServiceUnitOfWork = tPServiceUnitOfWork;
        }

        #region DeleteAsync
        public Task DeleteAsync(int id) => _repositoryUnitOfWork.BusinessCard.Value.DeleteAsync(id);
        #endregion

        #region GetAllAsync
        public Task<PaginatedResult<BusinessCard>> GetAllAsync(int pageNumber, int pageSize) => _repositoryUnitOfWork.BusinessCard.Value.GetAllAsync(pageNumber, pageSize);
        #endregion

        #region GetByIdAsync
        public Task<BusinessCard> GetByIdAsync(int id) => _repositoryUnitOfWork.BusinessCard.Value.GetByIdAsync(id);
        #endregion

        #region CreateBusinessCard
        public async Task CreateBusinessCard(BusinessCardDto businessCardDto, IFormFile file)
        {
            var businessCard = MapDtoToEntity(businessCardDto);
            await HandleAttachmentAsync(businessCard, file);
            await _repositoryUnitOfWork.BusinessCard.Value.AddAsync(businessCard);
        }
        #endregion

        #region GetBusinessCardsById
        public async Task<dynamic> GetBusinessCardsById(int id)
        {
            var businessCard = await GetByIdAsync(id);

            return new
            {
                businessCard.Id,
                businessCard.Name,
                businessCard.Gender,
                businessCard.DateOfBirth,
                businessCard.Email,
                businessCard.Phone,
                businessCard.Address,
                Attachments = businessCard.Attachments.Select(a => new
                {
                    a.Id,
                    a.FileName,
                    FileDataBase64 = a.FileData != null ? Convert.ToBase64String(a.FileData) : null,
                    a.BusinessCardId
                }).ToList()
            };
        }
        #endregion

        #region FilterBusinessCardsAsync
        public Task<PaginatedResult<BusinessCard>> FilterBusinessCardsAsync(string name, string email, string phone, string gender, DateTime? dob, int pageNumber, int pageSize)
        {
            return _repositoryUnitOfWork.BusinessCard.Value.FilterAsync(name, email, phone, gender, dob, pageNumber, pageSize);
        }
        #endregion

        #region ExportBusinessCardsAsync
        public async Task<byte[]> ExportBusinessCardsAsync(string format, int id)
        {
            var businessCard = await GetByIdAsync(id);
            return format.ToLower() switch
            {
                "csv" => ExportToCsv(businessCard),
                "xml" => ExportToXml(businessCard),
                _ => throw new ArgumentException("Invalid export format."),
            };
        }
        #endregion

        #region ValidateBusinessCardImport
        public async Task<(List<string> errors, BusinessCard validData)> ValidateBusinessCardImport(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file uploaded");

            string fileContent;
            using (var streamReader = new StreamReader(file.OpenReadStream()))
            {
                fileContent = await streamReader.ReadToEndAsync();
            }

             BusinessCard businessCard;
            if (file.FileName.EndsWith(".csv"))
            {
                businessCard = ParseCsv(fileContent);
            }
            else if (file.FileName.EndsWith(".xml"))
            {
                businessCard = ParseXml(fileContent);
            }
            else
            {
                throw new ArgumentException("Invalid file format.");
            }

            var errors = ValidateBusinessCards(businessCard);
            if (errors.Count > 0)
            {
                return (errors, null);
            }

            var validData = businessCard;
            return (null, validData);
        }
        #endregion



        #region Helper Method

        #region MapDtoToEntity
        private BusinessCard MapDtoToEntity(BusinessCardDto dto)
        {
            return new BusinessCard
            {
                Name = dto.Name,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                CreationUser = dto.CreationUser,
                CreationDate = dto.CreationDate,
                Attachments = new List<Attachment>()
            };
        }
        #endregion

        #region HandleAttachmentAsync
        private async Task HandleAttachmentAsync(BusinessCard businessCard, IFormFile file)
        {
            if (file == null || file.Length == 0) return;

            var attachment = await CreateAttachmentAsync(file, businessCard);
            businessCard.Attachments.Add(attachment);
        }
        #endregion

        #region CreateAttachmentAsync
        private async Task<Attachment> CreateAttachmentAsync(IFormFile file, BusinessCard businessCard)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            return new Attachment
            {
                FileName = file.FileName,
                FileData = memoryStream.ToArray(),
                BusinessCard = businessCard,
                CreationUser = businessCard.CreationUser,
                CreationDate = businessCard.CreationDate
            };
        }
        #endregion

        #region ExportToCsv
        private byte[] ExportToCsv(BusinessCard businessCard)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Name,Gender,DateOfBirth,Email,Phone,Address");
            csv.AppendLine($"{businessCard.Name},{businessCard.Gender},{businessCard.DateOfBirth},{businessCard.Email},{businessCard.Phone},{businessCard.Address}");
            return Encoding.UTF8.GetBytes(csv.ToString());
        }
        #endregion

        #region ExportToXml
        private byte[] ExportToXml(BusinessCard businessCard)
        {
            var xml = new StringBuilder();
            xml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.AppendLine("<BusinessCard>");
            xml.AppendLine($"  <Name>{businessCard.Name}</Name>");
            xml.AppendLine($"  <Gender>{businessCard.Gender}</Gender>");
            xml.AppendLine($"  <DateOfBirth>{businessCard.DateOfBirth}</DateOfBirth>");
            xml.AppendLine($"  <Email>{businessCard.Email}</Email>");
            xml.AppendLine($"  <Phone>{businessCard.Phone}</Phone>");
            xml.AppendLine($"  <Address>{businessCard.Address}</Address>");
            xml.AppendLine("</BusinessCard>");

            return Encoding.UTF8.GetBytes(xml.ToString());
        }
        #endregion

        #region ParseCsv

        private BusinessCard ParseCsv(string csvContent)
        {
            var businessCard = new BusinessCard();
            var rows = csvContent.Split('\n').Skip(1);

            foreach (var row in rows)
            {
                if (!string.IsNullOrWhiteSpace(row))
                {
                    var columns = row.Split(',');
                    businessCard = new BusinessCard
                    {
                        Name = columns[0],
                        Gender = columns[1],
                        DateOfBirth = DateTime.Parse(columns[2]),
                        Email = columns[3],
                        Phone = columns[4],
                        Address = columns[5]
                    };
                }
            }

            return businessCard;
        }
        #endregion

        #region ParseXml

        private BusinessCard ParseXml(string xmlContent)
        {
            var serializer = new XmlSerializer(typeof(BusinessCard));
            var xmlReaderSettings = new XmlReaderSettings
            {
                IgnoreWhitespace = true 
            };

            using (var stringReader = new StringReader(xmlContent))
            using (var xmlReader = XmlReader.Create(stringReader, xmlReaderSettings))
            {
                return (BusinessCard)serializer.Deserialize(xmlReader);
            }
        }
        #endregion


        #region ValidateBusinessCards

        private List<string> ValidateBusinessCards(BusinessCard card)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(card.Name) || card.Name.Length > 100)
                errors.Add("Name is required and must be less than 100 characters.");

            if (string.IsNullOrWhiteSpace(card.Gender) || (card.Gender != "Male" && card.Gender != "Female"))
                errors.Add("Gender is required and must be either 'Male' or 'Female'.");

            if (string.IsNullOrWhiteSpace(card.Address) || card.Address.Length > 50)
                errors.Add("Address is required and must be less than 50 characters.");

            if (!IsValidEmail(card.Email))
                errors.Add("Invalid email address.");

            if (card.DateOfBirth == default(DateTime))
            {
                errors.Add("Date of Birth is required.");
            }
            else if (card.DateOfBirth > DateTime.Now)
            {
                errors.Add("Date of Birth cannot be in the future.");
            }
            else if (card.DateOfBirth < new DateTime(1900, 1, 1))
            {
                errors.Add("Date of Birth must be later than January 1, 1900.");
            }

            return errors;
        }
        #endregion

        #region IsValidEmail

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #endregion

    }
}
