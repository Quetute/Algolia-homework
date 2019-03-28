using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Algolia.Controllers
{
    [Route("1/[controller]")]
    [ApiController]
    public class QueriesController : ControllerBase
    {
        public IQueriesProcessor QueriesProcessor { get; }

        public QueriesController(IQueriesProcessor queriesProcessor)
        {
            QueriesProcessor = queriesProcessor;
        }

        [HttpGet("count/{datePrefix}")]
        public ActionResult Get(string datePrefix)
        {
            DateTime startDate;
            DateTime endDate;
            try 
            {
                (startDate, endDate) = GetTimeRange(datePrefix);                    
            }
            catch(ArgumentException){
                return BadRequest("Invalid Date Prefix format");
            } 
            return Ok(new {count = QueriesProcessor.GetCount(startDate, endDate)});
        }

        [HttpGet("popular/{datePrefix}")]
        public ActionResult Get(string datePrefix, [FromQuery] int size)
        {
            DateTime startDate;
            DateTime endDate;
            try 
            {
                (startDate, endDate) = GetTimeRange(datePrefix);                    
            }
            catch(ArgumentException){
                return BadRequest("Invalid Date Prefix format");
            }
            return Ok(new {queries = QueriesProcessor.GetPopular(startDate, endDate, size).Select(kv => new {query = kv.Key, count = kv.Value})});
        }

        private (DateTime startDate, DateTime endDate) GetTimeRange(string datePrefix){
            DateTime startDate;
            if(DateTime.TryParseExact(datePrefix, "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,out startDate)){
                return (startDate, startDate.AddYears(1));
            }

            if(DateTime.TryParseExact(datePrefix, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None,out startDate)){
                return (startDate, startDate.AddMonths(1));
            }

            if(DateTime.TryParseExact(datePrefix, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None,out startDate)){
                return (startDate, startDate.AddDays(1));
            }

            if(DateTime.TryParseExact(datePrefix, "yyyy-MM-dd HH", CultureInfo.InvariantCulture, DateTimeStyles.None,out startDate)){
                return (startDate, startDate.AddHours(1));
            }

            if(DateTime.TryParseExact(datePrefix, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None,out startDate)){
                return (startDate, startDate.AddMinutes(1));
            }

            if(DateTime.TryParseExact(datePrefix, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None,out startDate)){
                return (startDate, startDate.AddSeconds(1));
            }

            throw new ArgumentException("Unexpected format");
        }
    }
}
