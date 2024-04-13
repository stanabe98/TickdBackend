using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Text;
using TickdBackend.Application.Controllers;
using TickdBackend.Application.Interfaces.Services;
using TickdBackend.Application.Models.Application;

namespace TickdBackend.Test.Controllers
{

    [TestFixture]
    public class TickdControllerTest
    {
        private Mock<ITickdService> _tickdService;
        private TickdController _controller;

        [SetUp]
        public void Setup()
        {
            _tickdService = new Mock<ITickdService>();
            _controller = new TickdController(_tickdService.Object);
        }

        [Test]
        public async Task UploadCSV_ValidCSV_ReturnsSuccess()
        {
            // Arrange
            string csvContent = @"AccountId,MeterReadingDateTime,MeterReadValue
            1,22/04/2019 12:25:00,100
            2,22/04/2019 12:45:00,200
            3,22/04/2019 12:35:00,300";

            var fileMock = new Mock<IFormFile>();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
            fileMock.Setup(_ => _.OpenReadStream()).Returns(stream);
            fileMock.Setup(_ => _.Length).Returns(stream.Length);
            fileMock.Setup(_ => _.ContentType).Returns("text/csv");

            var expectedResponse = new ApplicationResponse()
            {
                failedReadings = 0,
                successfulReadings = 3
            };
            _tickdService.Setup(x => x.GetMeterReadings(It.IsAny<List<MeterReadingCsv>>())).ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.UploadCSV(fileMock.Object);

            // Assert
            Assert.That(result.Value, Is.EqualTo(expectedResponse));
        }


        [Test]
        public async Task UploadCSV_Not_CSV_Uploaded_BadRequest()
        {
            // Arrange

            string csvContent = @"AccountId,MeterReadingDateTime,MeterReadValue
            1,22/04/2019 12:25:00,100
            2,22/04/2019 12:45:00,200
            3,22/04/2019 12:35:00,300";

            var fileMock = new Mock<IFormFile>();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
            fileMock.Setup(_ => _.OpenReadStream()).Returns(stream);
            fileMock.Setup(_ => _.Length).Returns(stream.Length);
            fileMock.Setup(_ => _.ContentType).Returns("application/pdf");

            // Act
            var result = await _controller.UploadCSV(fileMock.Object);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var errorResponse = ((BadRequestObjectResult)result.Result).Value;
            var errorCode = errorResponse.GetType().GetProperty("code").GetValue(errorResponse);
            Assert.That(errorCode, Is.EqualTo(410));
        }

        [Test]
        public async Task UploadCSV_WrongDateFormat_Returns_BadRequest()
        {
            // Arrange

            string csvContent = @"AccountId,MeterReadingDateTime,MeterReadValue
            1,xde 12:25:00,100
            2,22/04/2019 12:45:00,200
            3,22/04/2019 12:35:00,300";

            var fileMock = new Mock<IFormFile>();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
            fileMock.Setup(_ => _.OpenReadStream()).Returns(stream);
            fileMock.Setup(_ => _.Length).Returns(stream.Length);
            fileMock.Setup(_ => _.ContentType).Returns("text/csv");

            // Act
            var result = await _controller.UploadCSV(fileMock.Object);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var errorResponse = ((BadRequestObjectResult)result.Result).Value;
            var errorCode = errorResponse.GetType().GetProperty("code").GetValue(errorResponse);
            Assert.That(errorCode, Is.EqualTo(420));
        }

        [Test]
        public async Task UploadCSV_IncompatibleTableColumns_Returns_BadRequest()
        {
            // Arrange

            string csvContent = @"Id,DateTime,ReadValue
            1,22/04/2019 12:25:00,100
            2,22/04/2019 12:45:00,200
            3,22/04/2019 12:35:00,300";

            var fileMock = new Mock<IFormFile>();
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));
            fileMock.Setup(_ => _.OpenReadStream()).Returns(stream);
            fileMock.Setup(_ => _.Length).Returns(stream.Length);
            fileMock.Setup(_ => _.ContentType).Returns("text/csv");

            // Act
            var result = await _controller.UploadCSV(fileMock.Object);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            var errorResponse = ((BadRequestObjectResult)result.Result).Value;
            var errorCode = errorResponse.GetType().GetProperty("code").GetValue(errorResponse);
            Assert.That(errorCode, Is.EqualTo(430));
        }
    }

}