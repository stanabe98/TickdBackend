namespace TickdBackend.Application.Models.Application
{
    public class UserMeterReadingResponse
    {
        public int AccountId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MeterReadingDateTime { get; set; }
        public int? MeterReadValue { get; set; }
    }
}
