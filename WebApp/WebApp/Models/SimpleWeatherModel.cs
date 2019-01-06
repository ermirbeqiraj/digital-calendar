using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class SimpleWeatherModel
    {
        public string Time { get; set; }
        public int Temp { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }
}
