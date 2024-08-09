using InterviewProject.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InterviewProject.Services.Interfaces
{
    public interface IWeatherForecastService
    {
        Task<List<WeatherForecast>> GetWeatherForecastAsync(string location);
    }
}