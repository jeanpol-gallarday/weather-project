using System;
using System.Collections.Generic;

namespace InterviewProject.Controllers.Contracts
{
    public class main
    {
        public double temp { get; set; }
    }

    public class weather
    {
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    public class forecast
    {
        public main main { get; set; }
        public List<weather> weather { get; set; }
        public DateTime dt_txt { get; set; }
    }

    public class WeatherForecastRoot
    {
        public int cnt { get; set; }
        public List<forecast> list { get; set; }
    }
}