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
        }
    }
}