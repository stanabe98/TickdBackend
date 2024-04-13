using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using CsvHelper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace TickdBackend.Application.Models.Database
{

    [Table("MeterReadings")]
    public class MeterReadings
    {
        [Column("AccountId")]
        public int AccountId { get; set; }
        [Column("MeterReadingDateTime")]
        public DateTime MeterReadingDateTime { get; set; }

        [Column("MeterReadValue")]
        public int? MeterReadValue { get; set; }
    }


}
