using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public async Task<IActionResult> Index()
        {
            return View();
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
