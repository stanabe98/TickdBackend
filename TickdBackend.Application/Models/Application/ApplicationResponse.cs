namespace TickdBackend.Application.Models.Application
{
    public class ApplicationResponse
    {
        //public List<MeterReadingCsv> meterReadingCsvs { get; set; } = new List<MeterReadingCsv>();
        public int successfulReadings { get; set; }
        public int failedReadings { get; set;}
    }
}
