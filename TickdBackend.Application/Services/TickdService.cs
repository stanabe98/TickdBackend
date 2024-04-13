using AutoMapper;
using TickdBackend.Application.Interfaces.Repositories;
using TickdBackend.Application.Interfaces.Services;
using TickdBackend.Application.Models.Application;

namespace TickdBackend.Application.Services
{
    public class TickdService:ITickdService
    {
        private readonly ITickdRepository _repository;
        private readonly IMapper _mapper;
        public TickdService(ITickdRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task DeleteMeterReadings()
        {
            await _repository.DeleteMeterReadingsAsync();
        }

        public async Task<List<UserMeterReadingResponse>> GetUserMeterReadings()
        {
            var userMeterReadings= await _repository.GetUserMeterReadingsAsync();
            var result = _mapper.Map<List<UserMeterReadingResponse>>(userMeterReadings);

            return result;
        }

        public async Task<List<MeterReadingResponse>> GetMeterReadings()
        {
            var MeterReadings = await _repository.GetMeterReadingsAsync();
            var result= _mapper.Map<List<MeterReadingResponse>>(MeterReadings);

            return result;
        }

        public async Task<ApplicationResponse> GetMeterReadings(List<MeterReadingCsv> readings)
        {
            //grouping the meter readings by accountId, incase of duplicates
            //selecting the latest by ordering in ordering in descending order and selecting the first
            //eliminating the entries with meterReadValue of null
            var filteredReadings = readings.Where(s=>s.MeterReadValue!=null)
            .GroupBy(r => r.AccountId)
            .Select(g => g.OrderByDescending(r => r.MeterReadingDateTime).First())
            //.Where(s=>s.MeterReadValue != null)
            .ToList();

            var result= await _repository.GetMeterReadingsAsync(filteredReadings);
            var failedReadings = readings.Count - result.successfulReadings;

            return new ApplicationResponse
            {
                successfulReadings= result.successfulReadings, failedReadings= failedReadings
            };
        }
    }
}
