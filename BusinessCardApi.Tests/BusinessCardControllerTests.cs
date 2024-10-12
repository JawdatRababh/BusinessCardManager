using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using API.Controllers;
using Service.Interfaces;
using Domain.Models.BusinessCardDto;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using FluentAssertions;
using System.Collections.Generic;
using System;
using Domain.Models;

namespace BusinessCardApi.Tests
{
    public class BusinessCardControllerTests
    {
        private readonly BusinessCardController _controller;
        private readonly Mock<IServiceUnitOfWork> _serviceUnitOfWorkMock;
        private readonly Mock<IBusinessCardService> _businessCardServiceMock;
        private readonly Mock<ILogger<BusinessCardController>> _loggerMock;
        private Lazy<IBusinessCardService> _lazyBusinessCardService;

        public BusinessCardControllerTests()
        {
            _businessCardServiceMock = new Mock<IBusinessCardService>();
            _serviceUnitOfWorkMock = new Mock<IServiceUnitOfWork>();
            _loggerMock = new Mock<ILogger<BusinessCardController>>();

            var lazyBusinessCardService = new Lazy<IBusinessCardService>(() => _businessCardServiceMock.Object);
            _serviceUnitOfWorkMock.Setup(s => s.BusinessCard).Returns(lazyBusinessCardService);

            _controller = new BusinessCardController(_serviceUnitOfWorkMock.Object, null, null, null, _loggerMock.Object);
        }

        #region GetBusinessCards_ShouldReturnOkResult
        [Fact]
        public async Task GetBusinessCards_ShouldReturnOkResult()
        {

            var paginatedResult = new PaginatedResult<BusinessCard>
            {
                Data = new List<BusinessCard>
                {
                    new BusinessCard
                    {
                        Name = "Jawdat",
                        Gender = "Male",
                        DateOfBirth = new DateTime(1990, 1, 1),
                        Email = "Jawdat@example.com",
                        Phone = "123456789",
                        Address = "123 Main Street",
                        CreationUser = "Admin",
                        CreationDate = DateTime.UtcNow
                    }
                },
                TotalCount = 1
            };

            _businessCardServiceMock.Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(paginatedResult);

            var result = await _controller.GetBusinessCards(1, 5);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(paginatedResult);
        }
        #endregion

        #region CreateBusinessCard_ShouldReturnOkResult_WhenModelIsValid

        [Fact]
        public async Task CreateBusinessCard_ShouldReturnOkResult_WhenModelIsValid()
        {
            var businessCardDto = new BusinessCardDto
            {
                Name = "Jawdat",
                Email = "Jawdat@example.com",
                Gender = "Male"
            };

            _businessCardServiceMock.Setup(s => s.CreateBusinessCard(It.IsAny<BusinessCardDto>(), It.IsAny<IFormFile>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.CreateBusinessCard(businessCardDto, null);

            result.Should().BeOfType<OkResult>();
        }
        #endregion

        #region DeleteBusinessCard_ShouldReturnNoContent_WhenSuccess

        [Fact]
        public async Task DeleteBusinessCard_ShouldReturnNoContent_WhenSuccess()
        {
            var businessCardId = 1;
            _businessCardServiceMock.Setup(s => s.DeleteAsync(businessCardId))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeleteBusinessCard(businessCardId);

            result.Should().BeOfType<NoContentResult>();
        }
        #endregion

        #region GetBusinessCardsById_ShouldReturnOkResult_WhenCardExists

        [Fact]
        public async Task GetBusinessCardsById_ShouldReturnOkResult_WhenCardExists()
        {
            var businessCard = new BusinessCard
            {
                Name = "Jawdat",
                Gender = "Male",
                DateOfBirth = new DateTime(1990, 1, 1),
                Email = "Jawdat@example.com",
                Phone = "123456789",
                Address = "123 Main Street",
                CreationUser = "Admin",
                CreationDate = DateTime.UtcNow
            };

            _businessCardServiceMock.Setup(s => s.GetBusinessCardsById(It.IsAny<int>()))
                .ReturnsAsync(businessCard);

            var result = await _controller.GetBusinessCardsById(1);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(businessCard);
        }
        #endregion

        #region FilterBusinessCards_ShouldReturnOkResult_WithFilteredData

        [Fact]
        public async Task FilterBusinessCards_ShouldReturnOkResult_WithFilteredData()
        {
            var filteredResult = new PaginatedResult<BusinessCard>
            {
                Data = new List<BusinessCard>
                {
                    new BusinessCard
                    {
                        Name = "FilteredName",
                        Gender = "Male",
                        DateOfBirth = new DateTime(1990, 1, 1),
                        Email = "filtered@example.com",
                        Phone = "123456789",
                        Address = "456 Filtered Street",
                        CreationUser = "Admin",
                        CreationDate = DateTime.UtcNow
                    }
                },
                TotalCount = 1
            };

            _businessCardServiceMock.Setup(s => s.FilterBusinessCardsAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(filteredResult);

            var result = await _controller.FilterBusinessCards("FilteredName", null, null, null, null, 1, 5);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(filteredResult);
        }
        #endregion

        #region ExportBusinessCards_ShouldReturnFileResult_WhenValidFormat

        [Fact]
        public async Task ExportBusinessCards_ShouldReturnFileResult_WhenValidFormat()
        {
            var businessCardId = 1;
            var fileFormat = "csv";
            var fileData = new byte[] { 1, 2, 3 };
            _businessCardServiceMock.Setup(s => s.ExportBusinessCardsAsync(fileFormat, businessCardId))
                .ReturnsAsync(fileData);

            var result = await _controller.ExportBusinessCards(fileFormat, businessCardId);

            var fileResult = result.Should().BeOfType<FileContentResult>().Subject;
            fileResult.FileContents.Should().BeEquivalentTo(fileData);
            fileResult.ContentType.Should().Be("text/csv");
            fileResult.FileDownloadName.Should().Be("business_card.csv");
        }
        #endregion

        #region ValidateBusinessCardImport_ShouldReturnOkWithValidData

        [Fact]
        public async Task ValidateBusinessCardImport_ShouldReturnOkWithValidData()
        {
            var mockFile = new Mock<IFormFile>();
            var content = "Name,Gender,DateOfBirth,Email,Phone,Address";
            var fileName = "businesscard.csv";
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            writer.Write(content);
            writer.Flush();
            memoryStream.Position = 0;

            mockFile.Setup(f => f.OpenReadStream()).Returns(memoryStream);
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(memoryStream.Length);

            var validBusinessCard = new BusinessCard
            {
                Name = "Jawdat",
                Gender = "Male",
                DateOfBirth = new System.DateTime(1990, 1, 1),
                Email = "Jawdat.Rab@example.com",
                Phone = "1234567890",
                Address = "123 Main St"
            };

            _businessCardServiceMock
                .Setup(s => s.ValidateBusinessCardImport(It.IsAny<IFormFile>()))
                .ReturnsAsync((null, validBusinessCard));

            var result = await _controller.ValidateBusinessCardImport(mockFile.Object) as OkObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);

            var resultValue = result.Value.GetType().GetProperty("data").GetValue(result.Value, null);
            resultValue.Should().NotBeNull(because: "the API should return valid data.");
        }
        #endregion

    }

}

