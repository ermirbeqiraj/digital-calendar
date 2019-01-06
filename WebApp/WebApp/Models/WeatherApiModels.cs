using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class WeatherApiResponseModel
    {
        public string Cod { get; set; }
        public float Message { get; set; }
        public int Cnt { get; set; }
        public City City { get; set; }
        public List<WeatherListItem> List { get; set; } = new List<WeatherListItem>();
    }

    public class WeatherListItem
    {
        public int Dt { get; set; }
        public string Dt_txt { get; set; }
        //public Snow Snow { get; set; }
        public Wind Wind { get; set; }
        public Clouds Clouds { get; set; }
        public List<WeatherItem> Weather { get; set; } = new List<WeatherItem>();
        public Main Main { get; set; }
    }

    public class Main
    {
        public float Temp { get; set; }
        public float Temp_min { get; set; }
        public float Temp_max { get; set; }
        public float Pressure { get; set; }
        public float Sea_level { get; set; }
        public float Grnd_level { get; set; }
        public int Humidity { get; set; }
        public float Temp_kf { get; set; }
    }
    
    public class WeatherItem
    {
        public int Id { get; set; }
        public string Main { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }

    public class Clouds
    {
        public int All { get; set; }
    }

    public class Wind
    {
        public float Speed { get; set; }
        public float Deg { get; set; }
    }

    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Coord Coord { get; set; }
        public string Country { get; set; }
    }

    public class Coord
    {
        public float Lat { get; set; }
        public float Lon { get; set; }
    }
}
