using InterviewProject.Controllers.Contracts;
using System.Threading.Tasks;

namespace InterviewProject.Services.Interfaces
{
    public interface IWeatherHttpService
    {
        Task<WeatherForecastRoot?> GetWeatherForecastAsync(string location);
    }
}