using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TickdBackend.Application.Context;
using TickdBackend.Application.Interfaces.Repositories;
using TickdBackend.Application.Models.Application;
using TickdBackend.Application.Models.Database;

namespace TickdBackend.Application.Repositories
{
    public class TickdRepository:ITickdRepository
    {
        private readonly TickdDbContext _context;

        public TickdRepository(TickdDbContext context)
        {
            _context = context;
        }

        public async Task DeleteMeterReadingsAsync()
        {
            await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"MeterReadings\"");
          
        }

        public async Task<List<UserMeterReadings>> GetUserMeterReadingsAsync()
        {
            var userMeterReadings = await _context.UserMeterReadings.ToListAsync();

            if(userMeterReadings != null)
            {
            return userMeterReadings;

            }
            return new List<UserMeterReadings>();
        }

        public async Task<List<MeterReadings>> GetMeterReadingsAsync()
        {
            var MeterReadings = await _context.MeterReadings.OrderBy(s=>s.AccountId).ToListAsync();

            if (MeterReadings != null)
            {
                return MeterReadings;

            }
            return new List<MeterReadings>();
        }

        public async Task<ApplicationResponse> GetMeterReadingsAsync(List<MeterReadingCsv> readings)
        {
            var validAccountIds= await _context.TickdUsers.Select(u=>u.AccountId).ToListAsync();
            var validRecords= readings.Where(r=>validAccountIds.Contains(r.AccountId));

            var successCount = 0;

            foreach (var record in validRecords)
            {
               
                var existingRecord = _context.MeterReadings?.FirstOrDefault(r => r.AccountId == record.AccountId);

                if (existingRecord != null)
                {
                    _context.Entry(existingRecord).State = EntityState.Detached;
                    if (record.MeterReadingDateTime >= existingRecord.MeterReadingDateTime)
                    {
                        if (record.MeterReadValue == existingRecord.MeterReadValue && record.MeterReadingDateTime == existingRecord.MeterReadingDateTime)
                        {
                            continue;
                        }
                        existingRecord.MeterReadValue = record.MeterReadValue;
                        existingRecord.MeterReadingDateTime = record.MeterReadingDateTime.ToUniversalTime();
                        //_context.MeterReadings.Update(existingRecord);
                        successCount++;
                    }
                }
                else
                {
                    var newRecord = new MeterReadings
                    {
                        MeterReadingDateTime = record.MeterReadingDateTime.ToUniversalTime(),
                        AccountId = record.AccountId,
                        MeterReadValue = record.MeterReadValue
                    };
                    _context.MeterReadings?.Add(newRecord);
                    successCount++;
                }

            }
            await _context.SaveChangesAsync();

            return new ApplicationResponse
            {
                successfulReadings = successCount,
            };
        }
    }
}
