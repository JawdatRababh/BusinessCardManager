using DocumentFormat.OpenXml.Office2010.Excel;
using Domain.Models.BusinessCardDto;
using Domin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.Services;
using Service.UnitOfWork;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessCardController : BaseController
    {
        private readonly IServiceUnitOfWork _serviceUnitOfWork;
        private readonly IConfiguration _configuration;
        private IHttpContextAccessor _httpContextAccessor;
        private ITPServiceUnitOfWork _tPServiceUnitOfWork;
        private readonly ILogger<BusinessCardController> _logger;

        public BusinessCardController(IServiceUnitOfWork serviceUnitOfWork,
            ITPServiceUnitOfWork tPServiceUnitOfWork,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<BusinessCardController> logger)
            : base(serviceUnitOfWork, tPServiceUnitOfWork, configuration, httpContextAccessor, logger)
        {
            _serviceUnitOfWork = serviceUnitOfWork;
            _logger = logger;
        }

        [HttpGet("GetBusinessCards")]
        public async Task<IActionResult> GetBusinessCards([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                var result = await _serviceUnitOfWork.BusinessCard.Value.GetAllAsync(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching business cards. Message: {Message}, StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode(500, ex.Message); 
            }
        }

        [HttpPost("CreateBusinessCard")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateBusinessCard([FromForm] BusinessCardDto businessCardDto, IFormFile file)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _serviceUnitOfWork.BusinessCard.Value.CreateBusinessCard(businessCardDto, file);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating business card. Message: {Message}, StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode(500, ex.Message); 
            }
        }

        [HttpDelete("DeleteBusinessCard/{id}")]
        public async Task<IActionResult> DeleteBusinessCard(int id)
        {
            try
            {
                await _serviceUnitOfWork.BusinessCard.Value.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting business card with ID {Id}. Message: {Message}, StackTrace: {StackTrace}", id, ex.Message, ex.StackTrace);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetBusinessCardsById/{id}")]
        public async Task<IActionResult> GetBusinessCardsById(int id)
        {
            try
            {
                var businessCard = await _serviceUnitOfWork.BusinessCard.Value.GetBusinessCardsById(id);
                return Ok(businessCard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching business card by ID {Id}. Message: {Message}, StackTrace: {StackTrace}", id, ex.Message, ex.StackTrace);
                return StatusCode(500, ex.Message); 
            }
        }

        [HttpGet("FilterBusinessCards")]
        public async Task<IActionResult> FilterBusinessCards([FromQuery] string? name = null, [FromQuery] string? email = null, [FromQuery] string? phone = null, [FromQuery] string? gender = null, [FromQuery] DateTime? dob = null, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                var result = await _serviceUnitOfWork.BusinessCard.Value.FilterBusinessCardsAsync(name, email, phone, gender, dob, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while filtering business cards. Message: {Message}, StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode(500, ex.Message); 
            }
        }

        [HttpGet("ExportBusinessCards/{format}/{id}")]
        public async Task<IActionResult> ExportBusinessCards(string format, int id)
        {
            try
            {
                var fileData = await _serviceUnitOfWork.BusinessCard.Value.ExportBusinessCardsAsync(format, id);
                var contentType = format == "csv" ? "text/csv" : "application/xml";
                var fileName = format == "csv" ? "business_card.csv" : "business_card.xml";

                return File(fileData, contentType, fileName);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while exporting business card. Message: {Message}, StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return BadRequest(ex.Message); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while exporting business card. Message: {Message}, StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode(500, ex.Message); 
            }
        }

        [HttpPost("ValidateBusinessCardImport")]
        public async Task<IActionResult> ValidateBusinessCardImport([FromForm] IFormFile file)
        {
            try
            {
                var (errors, validData) = await _serviceUnitOfWork.BusinessCard.Value.ValidateBusinessCardImport(file);

                if (errors != null && errors.Count > 0)
                {
                    return Ok(new { errors });
                }

                return Ok(new { data = validData });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while validating business card import. Message: {Message}, StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return BadRequest(ex.Message); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while validating business card import. Message: {Message}, StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode(500, ex.Message); 
            }
        }
    }
}


