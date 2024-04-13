using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Globalization;
using System.IO;
using TickdBackend.Application.Interfaces.Services;
using TickdBackend.Application.Models.Application;
using TickdBackend.Application.Models.Database;

namespace TickdBackend.Application.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class TickdController : ControllerBase
    {
        private readonly ITickdService _tickdService;
        public TickdController(ITickdService tickdService)
        {
            _tickdService = tickdService;
        }

        // demo purposes
        [HttpDelete("delete-meter-readings")]
        public async Task<ActionResult<String>> GetAllUsers()
        {
            try
            {
                await _tickdService.DeleteMeterReadings();

                return "trucated meter readings";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("user-meter-readings")]
        public async Task<ActionResult<List<UserMeterReadingResponse>>> GetUserMeterReadings()
        {
            try
            {
                var result = await _tickdService.GetUserMeterReadings();

                return result;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("meter-readings")]
        public async Task<ActionResult<List<MeterReadingResponse>>> GetMeterReadings()
        {
            try
            {
                var result = await _tickdService.GetMeterReadings();

                return result;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("meter-reading-uploads")]
        public async Task<ActionResult<ApplicationResponse>> UploadCSV(IFormFile file)
        {
            // checking if file is empty/file object is null and that it is a csv
            if (file == null || file.Length <= 0 || file.ContentType != "text/csv")
            {
                var errorResponse = new { code = 410, message = "Must upload file type CSV" };
                return BadRequest(errorResponse);
            }

            //List<MeterReadingCsv> emptyList = new List<MeterReadingCsv>();

            //using (TextFieldParser parser = new TextFieldParser(file.OpenReadStream()))
            //{
            //    parser.TextFieldType = FieldType.Delimited;
            //    parser.SetDelimiters(",");

            //    if (!parser.EndOfData)
            //    {
            //        parser.ReadLine();
            //    }
            //    // Read and parse each line of the CSV
            //    while (!parser.EndOfData)
            //    {
            //        string[] fields = parser.ReadFields();

            //        // Check if the row is empty or contains missing values
            //        if (fields == null || fields.Length < 3 || 
                        
            //           (string.IsNullOrWhiteSpace(fields[0]) && string.IsNullOrWhiteSpace(fields[1])
                       
                       
            //         ))
            //        {
            //            break; // Skip this row
            //        }

            //        string id = fields[0];
            //        string date = fields[1];
            //        string value = fields[2];

            //        MeterReadingCsv meterReading = new MeterReadingCsv
            //        {
            //            AccountId = int.Parse(id),
            //            MeterReadingDateTime = DateTime.Parse(date),
            //            MeterReadValue = int.TryParse(value, out int intValue) ? intValue : (int?)null
            //        };
            //        emptyList.Add(meterReading);
            //    }
            //}

            List<MeterReadingCsv> recordsList = new List<MeterReadingCsv>();


            using (var streamReader = new StreamReader(file.OpenReadStream()))
            using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
            {

                csvReader.Read();
                csvReader.ReadHeader();

                while (csvReader.Read())
                {

                    try
                    {
                        var record = csvReader.GetRecord<MeterReadingCsv>();
                        recordsList.Add(record);
                    }
                    catch (CsvHelperException ex)
                    {
                        var exceptionMessage = ex.InnerException?.Message;
                        if (exceptionMessage == "Wrong Date Type")
                        {
                            var dateError = new { code = 420, message = exceptionMessage };
                            return BadRequest(dateError);
                        }

                        var parsedFields = csvReader.Context.Reader.Parser.Record;
                        if (parsedFields != null)
                        {
                            if (parsedFields[0] == "" && parsedFields[1]=="" && parsedFields[1] == "")
                            {
                                break;
                            }
                        }
                        var something = ex.Data;                  
                        var errorResponse = new { code = 430, message = "Incompatible table columns" };
                        return BadRequest(errorResponse);             
                    }
                }
            }
            try
            {
                var response = await _tickdService.GetMeterReadings(recordsList);
                return response;
            }
            catch (Exception ex)
            {
                var errorResponse = new { code = 500, message = "Unknown Error" };
                return BadRequest(errorResponse);
            }


            //using (var reader = new StreamReader(file.OpenReadStream()))
            //{
            //    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            //    {


                //        // checking if the csv files has the neccessary columns by trying to parse into an array of objects of type MeterReadingCsv
                //        var emptyRecords = new List<MeterReadingCsv>();
                //        try
                //        {



                //                //parsing csv
                //                //csv.Configuration.RegisterClassMap<MeterReadingCsv>();
                //                var records = csv.GetRecords<MeterReadingCsv>().ToList();
                //            var response = await _tickdService.GetMeterReadings(records);
                //            return response;
                //        }
                //        catch (Exception ex)
                //        {
                //            var exceptionMessage = ex.InnerException?.Message;
                //            if (exceptionMessage == "Wrong Date Type")
                //            {
                //                var dateError = new { code = 420, message = exceptionMessage };
                //                return BadRequest(dateError);
                //            }

                //            var errorResponse = new { code = 430, message = "Incompatible table columns" };
                //            return BadRequest(errorResponse);
                //        }
                //    }
                //}
        }
    }
}