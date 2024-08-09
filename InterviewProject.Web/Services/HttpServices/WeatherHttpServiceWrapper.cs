using InterviewProject.Controllers.Contracts;
using InterviewProject.Services.Interfaces;
using System.Threading.Tasks;

namespace InterviewProject.Services.HttpServices
{
    public class WeatherHttpServiceWrapper : IWeatherHttpService
    {
        private readonly WeatherHttpService _weatherHttpService;

        public WeatherHttpServiceWrapper(WeatherHttpService weatherHttpService)
        {
            _weatherHttpService = weatherHttpService;
        }
        public Task<WeatherForecastRoot> GetWeatherForecastAsync(string location)
            => _weatherHttpService.GetWeatherForecastAsync(location);
    }
}