namespace TickdBackend.Application.Models.Application
{
    public class MeterReadingResponse
    {
        public int AccountId { get; set; }
        public string MeterReadingDateTime { get; set; }= string.Empty;
        public int? MeterReadValue { get; set; }
    }
}
