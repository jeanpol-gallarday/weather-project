using InterviewProject.Controllers.Contracts;
using InterviewProject.DTOs;
using System.Collections.Generic;

namespace InterviewProject.Mappers.Interfaces
{
    public interface IWeatherForecastMapper
    {
        List<WeatherForecast> Map(List<forecast> weatherForecastList);
    }
}