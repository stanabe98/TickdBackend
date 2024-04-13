using AutoMapper;
using TickdBackend.Application.Models.Application;
using TickdBackend.Application.Models.Database;

namespace TickdBackend.Application.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles() {

            CreateMap<TickdUsers, AllUsers>();
            CreateMap<MeterReadings, MeterReadingCsv>();
            CreateMap<UserMeterReadings, UserMeterReadingResponse>().ForMember(d=>d.MeterReadingDateTime,
                opt =>opt.MapFrom(s=>s.MeterReadingDateTime !=null ? 
                s.MeterReadingDateTime.Value.ToString("dd/MM/yyyy HH:mm:ss"):
                ""));
            CreateMap<MeterReadings, MeterReadingResponse>().ForMember(d => d.MeterReadingDateTime,
                opt => opt.MapFrom(s => s.MeterReadingDateTime != null ?
                s.MeterReadingDateTime.ToString("dd/MM/yyyy HH:mm:ss") :
                ""));
        }
    }
}
