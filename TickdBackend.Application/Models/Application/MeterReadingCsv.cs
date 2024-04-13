using CsvHelper.Configuration;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.TypeConversion;

namespace TickdBackend.Application.Models.Application
{
    public class MeterReadingCsv
    {
        //[Name("AccountId")]
        public int AccountId { get; set; }

        //[Name("MeterReadingDateTime")]
        [TypeConverter(typeof(CustomDateTimeConverter))]
        public DateTime MeterReadingDateTime { get; set; }
        //public string MeterReadingDateTime { get; set; }=string.Empty;

        //[Name("MeterReadValue")]
        [TypeConverter(typeof(CustomIntConverter))]
        public int? MeterReadValue { get; set; }
        //public string MeterReadValue { get; set; } = string.Empty;

    }

    public class CustomDateTimeConverter : DateTimeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            string[] formats = { "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy HH:mm" };
            if (DateTime.TryParseExact(text, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime result))
            {

                return result;
            }

            throw new Exception("Wrong Date Type");
        }
    }

    public class CustomIntConverter : DefaultTypeConverter
    {
        public override object? ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (int.TryParse(text, out int result))
            {
                if (result < 0)
                {
                    return null;
                }
                return result;
            }
            return null;
        }
    }
}
