using TickdBackend.Application.Models.Application;

namespace TickdBackend.Application.Interfaces.Services
{
    public interface ITickdService
    {
        Task DeleteMeterReadings();
        Task<List<UserMeterReadingResponse>> GetUserMeterReadings();
        Task<List<MeterReadingResponse>> GetMeterReadings();
        Task<ApplicationResponse> GetMeterReadings(List<MeterReadingCsv> readings);
    }
}
