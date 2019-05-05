using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace VehiclesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private string WeatherForecastJson = @"[
            {
            'date': '2018-05-06',
            'temperatureC': 1,
            'summary': 'Freezing',
            'temperatureF': 33
            },
            {
            'date': '2018-05-07',
            'temperatureC': 14,
            'summary': 'Bracing',
            'temperatureF': 57
            },
            {
            'date': '2018-05-08',
            'temperatureC': -13,
            'summary': 'Freezing',
            'temperatureF': 9
            },
            {
            'date': '2018-05-09',
            'temperatureC': -16,
            'summary': 'Balmy',
            'temperatureF': 4
            },
            {
            'date': '2018-05-10',
            'temperatureC': -2,
            'summary': 'Chilly',
            'temperatureF': 29
            }
        ]";

        // GET api/values
        [HttpGet]
        public ActionResult<WeatherForecast[]> Get()
        {
            return JsonConvert.DeserializeObject<WeatherForecast[]>(WeatherForecastJson); ;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF { get; set; }

        public string Summary { get; set; }
    }
}
