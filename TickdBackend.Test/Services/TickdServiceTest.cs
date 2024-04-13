using AutoMapper;
using Moq;
using NUnit.Framework;
using TickdBackend.Application.Interfaces.Repositories;
using TickdBackend.Application.Models.Application;
using TickdBackend.Application.Services;

namespace TickdBackend.Test.Services
{
    [TestFixture]
    public class TickdServiceTest
    {
        private Mock<ITickdRepository> _tickdRepository;
        private Mock<IMapper> _mapper;
        private TickdService _service;

        [SetUp]
        public void Setup()
        {
            _tickdRepository = new Mock<ITickdRepository>();
            _mapper = new Mock<IMapper>();
            _service = new TickdService(_tickdRepository.Object, _mapper.Object);
        }
        [Test]
        public async Task GetMeterReadings_Returns_SomeFailedReadings()
        {
            //Arrange
            var mockMeterReadings = new List<MeterReadingCsv>()
            {
                new MeterReadingCsv { AccountId = 1, MeterReadingDateTime = DateTime.Parse("2022-04-12 10:30:00"), MeterReadValue = 100 },
                new MeterReadingCsv { AccountId = 1, MeterReadingDateTime = DateTime.Parse("2022-04-12 10:30:00"), MeterReadValue = 100 },
                new MeterReadingCsv { AccountId = 2, MeterReadingDateTime = DateTime.Parse("2022-04-12 11:45:00"), MeterReadValue = 200 },
                new MeterReadingCsv { AccountId = 3, MeterReadingDateTime = DateTime.Parse("2022-04-12 12:15:00"), MeterReadValue = 300 },
                new MeterReadingCsv { AccountId = 3, MeterReadingDateTime = DateTime.Parse("2022-04-12 12:30:00"), MeterReadValue = null },
                new MeterReadingCsv { AccountId = 4, MeterReadingDateTime = DateTime.Parse("2022-04-12 12:45:00"), MeterReadValue = 100 },
                new MeterReadingCsv { AccountId = 5, MeterReadingDateTime = DateTime.Parse("2022-04-12 12:30:00"), MeterReadValue = null },
            };

            _tickdRepository.Setup(x => x.GetMeterReadingsAsync(It.IsAny<List<MeterReadingCsv>>())).ReturnsAsync((List<MeterReadingCsv> receivedReadings) =>
            {
                return new ApplicationResponse
                {
                    successfulReadings = receivedReadings.Count,
                    failedReadings = 0
                };
            });

            //Act
            var result = await _service.GetMeterReadings(mockMeterReadings);

            //Assert
            Assert.That(result, Is.Not.Null);
            // Verify expected counts
            // Duplicate accountIds are not stored, as they are filtered depending on whichever record has the latest reading 
            // Records with invalid MeterReads(null) are filtered
            Assert.That(result.successfulReadings, Is.EqualTo(4));
            Assert.That(result.failedReadings, Is.EqualTo(3));
        }

        [Test]
        public async Task GetMeterReadings_Returns_AllSaved_NoFailedReadings()
        {
            //Arrange
            var mockMeterReadings = new List<MeterReadingCsv>()
            {
                new MeterReadingCsv { AccountId = 1, MeterReadingDateTime = DateTime.Parse("2022-04-12 10:30:00"), MeterReadValue = 100 },
                new MeterReadingCsv { AccountId = 2, MeterReadingDateTime = DateTime.Parse("2022-04-12 11:45:00"), MeterReadValue = 200 },
                new MeterReadingCsv { AccountId = 3, MeterReadingDateTime = DateTime.Parse("2022-04-12 12:15:00"), MeterReadValue = 300 },
                new MeterReadingCsv { AccountId = 4, MeterReadingDateTime = DateTime.Parse("2022-04-12 12:45:00"), MeterReadValue = 100 },
                new MeterReadingCsv { AccountId = 5, MeterReadingDateTime = DateTime.Parse("2022-04-12 12:30:00"), MeterReadValue = 200 },
            };

            _tickdRepository.Setup(x => x.GetMeterReadingsAsync(It.IsAny<List<MeterReadingCsv>>())).ReturnsAsync((List<MeterReadingCsv> receivedReadings) =>
            {
                return new ApplicationResponse
                {
                    successfulReadings = receivedReadings.Count,
                    failedReadings = 0
                };
            });

            //Act
            var result = await _service.GetMeterReadings(mockMeterReadings);

            //Assert
            Assert.That(result, Is.Not.Null);
            // Verify expected counts
            // In this case all records have unique AccounId's and are meterReadValues are all valid Integars
            Assert.That(result.successfulReadings, Is.EqualTo(5));
        }

        // Scenario to mock possible existing records in Db with more recent MeterReads or records with invalid AccountIds;
        [Test]
        public async Task GetMeterReadings_Returns_FailedReadings_From_Repository()
        {
            //Arrange
            var mockMeterReadings = new List<MeterReadingCsv>()
            {
                new MeterReadingCsv { AccountId = 1, MeterReadingDateTime = DateTime.Parse("2022-04-12 10:30:00"), MeterReadValue = 100 },
                new MeterReadingCsv { AccountId = 2, MeterReadingDateTime = DateTime.Parse("2022-04-12 11:45:00"), MeterReadValue = 200 },
                new MeterReadingCsv { AccountId = 3, MeterReadingDateTime = DateTime.Parse("2022-04-12 12:15:00"), MeterReadValue = 300 },
                new MeterReadingCsv { AccountId = 4, MeterReadingDateTime = DateTime.Parse("2022-04-12 12:45:00"), MeterReadValue = 100 },
                new MeterReadingCsv { AccountId = 5, MeterReadingDateTime = DateTime.Parse("2022-04-12 12:30:00"), MeterReadValue = 200 },
            };

            _tickdRepository.Setup(x => x.GetMeterReadingsAsync(It.IsAny<List<MeterReadingCsv>>())).ReturnsAsync((List<MeterReadingCsv> receivedReadings) =>
            {
                return new ApplicationResponse
                {
                    successfulReadings = receivedReadings.Count - 2,
                    failedReadings = 2
                };
            });

            //Act
            var result = await _service.GetMeterReadings(mockMeterReadings);
            
            //Assert
            Assert.That(result, Is.Not.Null);
            // Verify expected counts
            Assert.That(result.successfulReadings, Is.EqualTo(3));
            Assert.That(result.failedReadings, Is.EqualTo(2));
        }
    }
}