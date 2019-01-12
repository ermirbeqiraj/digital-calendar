using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _configs;

        public HomeController(IHostingEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configs = configuration;
        }

        public IActionResult App()
        {
            return View();
        }

        [Route("~/api/[action]")]
        public IActionResult GetImages()
        {
            var provider = new PhysicalFileProvider(_env.WebRootPath);
            var files = provider.GetDirectoryContents("images");
            var fnames = files.Select(x => x.Name);
            return Ok(fnames);
        }

        [Route("~/api/[action]")]
        public async Task<IActionResult> GetWeather()
        {
            var openApiToken = _configs.GetValue<string>("OpenWeatherToken");
            var locationId = _configs.GetValue<string>("OpenWeatherCityId");
            var units = _configs.GetValue<string>("OpenWeatherUnits");
            var count = _configs.GetValue<int>("OpenWeatherCount");

            var weatherResponseModel = new WeatherApiResponseModel();
            var requestUrl = $"https://api.openweathermap.org/data/2.5/forecast?id={locationId}&appid={openApiToken}&units={units}&cnt={count}";
            using (var httpClient = new HttpClient())
            {
                var httpResponse = await httpClient.GetStringAsync(requestUrl);
                weatherResponseModel = JsonConvert.DeserializeObject<WeatherApiResponseModel>(httpResponse);
            }

            // convert to simple model
            var result = new List<SimpleWeatherModel>();
            foreach (var item in weatherResponseModel.List)
            {
                result.Add(new SimpleWeatherModel
                {
                    Time = DateTimeOffset.FromUnixTimeSeconds(item.Dt).DateTime.ToString("HH"),
                    Temp = (int)Math.Round(item.Main.Temp, 0, MidpointRounding.AwayFromZero),
                    Description = item.Weather.Select(x => x.Description).FirstOrDefault(),
                    Icon = item.Weather.Select(x => x.Icon).FirstOrDefault()
                });
            }
            // return
            return Ok(result);
        }

        [Route("~/api/[action]")]
        public async Task<IActionResult> GetEvents()
        {
            try
            {
                var provider = new PhysicalFileProvider(_env.WebRootPath);
                var file = provider.GetFileInfo(Path.Combine("calendar", "calendar-events.json"));
                var fileContent = "";

                if (!file.Exists)
                    return NotFound();

                using (var stream = file.CreateReadStream())
                using (var sr = new StreamReader(stream))
                {
                    fileContent = await sr.ReadToEndAsync();
                }

                if (string.IsNullOrEmpty(fileContent))
                    return BadRequest("No content.");

                var events = JsonConvert.DeserializeObject<List<StorageEvents>>(fileContent);
                var thisDayEvents = events.Where(x => x.Events.Any(e => e.When == DateTime.Now.ToString("yyyy-MM-dd")))
                                            .SelectMany(sm => sm.Events.Select(x => x.What));

                var holidays = events.Where(x => x.CalendarId == "en.al#holiday@group.v.calendar.google.com")
                                        .SelectMany(d => d.Events.Select(date => date.When));

                var fontModel = new
                {
                    today = thisDayEvents,
                    holidays = holidays
                };

                return Ok(fontModel);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                return BadRequest($"{ex.Message}:: {ex.StackTrace}");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
