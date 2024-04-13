namespace TickdBackend.Application.Models.Database
{
    
    public class UserMeterReadings
    {
        public int AccountId { get; set; }
        public string FirstName { get; set; }=string.Empty;
        public string LastName { get; set; }= string.Empty;
        public DateTime? MeterReadingDateTime { get; set; }
        public int? MeterReadValue { get; set; }
    }
}
