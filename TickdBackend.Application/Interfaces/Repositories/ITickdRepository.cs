using TickdBackend.Application.Models.Application;
using TickdBackend.Application.Models.Database;

namespace TickdBackend.Application.Interfaces.Repositories
{
    public interface ITickdRepository
    {
        Task DeleteMeterReadingsAsync();

        Task<ApplicationResponse> GetMeterReadingsAsync(List<MeterReadingCsv> readings);

        Task<List<UserMeterReadings>> GetUserMeterReadingsAsync();

        Task<List<MeterReadings>> GetMeterReadingsAsync();

    }
}
