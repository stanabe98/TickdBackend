using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TickdBackend.Application.Context;
using TickdBackend.Application.Interfaces.Repositories;
using TickdBackend.Application.Models.Application;
using TickdBackend.Application.Models.Database;
using TickdBackend.Application.Repositories;
using TickdBackend.Application.Services;

namespace TickdBackend.Test.Repositories
{
    [TestFixture]
    public class TickdRepositoryTest
    {
        private TickdDbContext _dbContext;
        private TickdRepository _repository;
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TickdDbContext>()
                .UseInMemoryDatabase(databaseName: "TickdTestDatabase")
                .Options;

            _dbContext = new TickdDbContext(options);
            _repository = new TickdRepository(_dbContext);
        }

        [TearDown]
        public async Task TearDown()
        {
            // Clean up the database after each test
            _dbContext.TickdUsers.RemoveRange(_dbContext.TickdUsers);
            _dbContext.MeterReadings.RemoveRange(_dbContext.MeterReadings);
            await _dbContext.SaveChangesAsync();
        }


        //SomeMeterReadings already exist, existingReadings are more recent than mockMeterReadings thus are not saved
        //2 new readings for accountId 4 and 5 added
        //AccountId 1 updated as different meterReadValue although same date
        [Test]
        public async Task GetMeterReadingsAsync_returns_some_readings_Saved()
        {

            var existingReadings = new List<MeterReadings>()
            {  new MeterReadings { AccountId = 1, MeterReadingDateTime = DateTime.Parse("2022-04-12 10:30:00"), MeterReadValue = 200 },
                new MeterReadings { AccountId = 2, MeterReadingDateTime = DateTime.Parse("2022-04-12 11:45:00"), MeterReadValue = 200 },
                new MeterReadings { AccountId = 3, MeterReadingDateTime = DateTime.Parse("2022-04-12 12:15:00"), MeterReadValue = 300 },
            };

            var existingAccounts = new List<TickdUsers>()
            {
                new TickdUsers{AccountId=1, FirstName="user1", LastName="test"},
                new TickdUsers{AccountId=2, FirstName="user2", LastName="test"},
                new TickdUsers{AccountId=3, FirstName="user3", LastName="test"},
                new TickdUsers{AccountId=4, FirstName="user4", LastName="test"},
                new TickdUsers{AccountId=5, FirstName="user5", LastName="test"},
            };

            var mockMeterReadings = new List<MeterReadingCsv>()
            {
                new MeterReadingCsv { AccountId = 1, MeterReadingDateTime = DateTime.Parse("2022-04-12 10:30:00"), MeterReadValue = 100 },
                new MeterReadingCsv { AccountId = 2, MeterReadingDateTime = DateTime.Parse("2022-04-12 11:45:00"), MeterReadValue = 200 },
                new MeterReadingCsv { AccountId = 3, MeterReadingDateTime = DateTime.Parse("2022-04-12 12:15:00"), MeterReadValue = 300 },
                new MeterReadingCsv { AccountId = 4, MeterReadingDateTime = DateTime.Parse("2022-04-12 12:45:00"), MeterReadValue = 100 },
                new MeterReadingCsv { AccountId = 5, MeterReadingDateTime = DateTime.Parse("2022-04-12 12:30:00"), MeterReadValue = 200 },
            };

            _dbContext.TickdUsers.AddRange(existingAccounts.ToList());
            _dbContext.MeterReadings.AddRange(existingReadings.ToList());
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetMeterReadingsAsync(mockMeterReadings);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.successfulReadings, Is.EqualTo(3));
        }

        //SomeMeterReadings already exist, mockMeterReadings are more recent than existingReadings thus are saved
        //2 new readings for accountId 4 and 5 added, 3 existing readings updated
        [Test]
        public async Task GetMeterReadingsAsync_all_readings_Saved()
        {
            var existingReadings = new List<MeterReadings>()
            {  new MeterReadings { AccountId = 1, MeterReadingDateTime = DateTime.Parse("2022-04-12 10:30:00"), MeterReadValue = 100 },
                new MeterReadings { AccountId = 2, MeterReadingDateTime = DateTime.Parse("2022-04-12 11:45:00"), MeterReadValue = 200 },
                new MeterReadings { AccountId = 3, MeterReadingDateTime = DateTime.Parse("2022-04-12 12:15:00"), MeterReadValue = 300 },
            };

            var existingAccounts = new List<TickdUsers>()
            {
                new TickdUsers{AccountId=1, FirstName="user1", LastName="test"},
                new TickdUsers{AccountId=2, FirstName="user2", LastName="test"},
                new TickdUsers{AccountId=3, FirstName="user3", LastName="test"},
                new TickdUsers{AccountId=4, FirstName="user4", LastName="test"},
                new TickdUsers{AccountId=5, FirstName="user5", LastName="test"},
            };

            var mockMeterReadings = new List<MeterReadingCsv>()
            {
                new MeterReadingCsv { AccountId = 1, MeterReadingDateTime = DateTime.Parse("2022-04-12 10:40:00"), MeterReadValue = 100 },
                new MeterReadingCsv { AccountId = 2, MeterReadingDateTime = DateTime.Parse("2022-04-12 11:55:00"), MeterReadValue = 200 },
                new MeterReadingCsv { AccountId = 3, MeterReadingDateTime = DateTime.Parse("2022-04-12 12:25:00"), MeterReadValue = 300 },
                new MeterReadingCsv { AccountId = 4, MeterReadingDateTime = DateTime.Parse("2022-04-12 12:45:00"), MeterReadValue = 100 },
                new MeterReadingCsv { AccountId = 5, MeterReadingDateTime = DateTime.Parse("2022-04-12 12:30:00"), MeterReadValue = 200 },
            };

            _dbContext.TickdUsers.AddRange(existingAccounts.ToList());
            _dbContext.MeterReadings.AddRange(existingReadings.ToList());
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetMeterReadingsAsync(mockMeterReadings);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.successfulReadings, Is.EqualTo(5));
        }

        //There are meterReadings with Invalid accountIds 7 & 8 these will not be saved
        //Reading for accountId 1 is updated 1 successfully
        [Test]
        public async Task GetMeterReadingsAsync_some_readings_Saved_inValid_accountIds()
        {
            var existingReadings = new List<MeterReadings>()
            {  new MeterReadings { AccountId = 1, MeterReadingDateTime = DateTime.Parse("2022-04-12 10:30:00"), MeterReadValue = 100 },
                new MeterReadings { AccountId = 2, MeterReadingDateTime = DateTime.Parse("2022-04-12 11:45:00"), MeterReadValue = 200 },
                new MeterReadings { AccountId = 3, MeterReadingDateTime = DateTime.Parse("2022-04-12 12:15:00"), MeterReadValue = 300 },
            };

            var existingAccounts = new List<TickdUsers>()
            {
                new TickdUsers{AccountId=1, FirstName="user1", LastName="test"},
                new TickdUsers{AccountId=2, FirstName="user2", LastName="test"},
                new TickdUsers{AccountId=3, FirstName="user3", LastName="test"},
                new TickdUsers{AccountId=4, FirstName="user4", LastName="test"},
                new TickdUsers{AccountId=5, FirstName="user5", LastName="test"},
            };

            var mockMeterReadings = new List<MeterReadingCsv>()
            {
                new MeterReadingCsv { AccountId = 1, MeterReadingDateTime = DateTime.Parse("2022-04-12 10:40:00"), MeterReadValue = 100 },
                new MeterReadingCsv { AccountId = 7, MeterReadingDateTime = DateTime.Parse("2022-04-12 11:55:00"), MeterReadValue = 200 },
                new MeterReadingCsv { AccountId = 8, MeterReadingDateTime = DateTime.Parse("2022-04-12 11:55:00"), MeterReadValue = 200 },
            };

            _dbContext.TickdUsers.AddRange(existingAccounts.ToList());
            _dbContext.MeterReadings.AddRange(existingReadings.ToList());
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetMeterReadingsAsync(mockMeterReadings);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.successfulReadings, Is.EqualTo(1));
        }
    }
}
