using InterviewProject.Controllers.Contracts;
using InterviewProject.DTOs;
using InterviewProject.Mappers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InterviewProject.Mappers
{
    public class WeatherForecastMapper : IWeatherForecastMapper
    {
        public List<WeatherForecast> Map(List<forecast> weatherForecastList)
        {
            var response = weatherForecastList.Select(Map);
            return response.ToList();
        }

        private WeatherForecast Map(forecast forecast)
            => new WeatherForecast()
            {
                Date = forecast.dt_txt,
                TemperatureC = Convert.ToInt32(forecast.main.temp),
                Summary = $"{forecast.weather?.FirstOrDefault()?.main?.ToString()}" +
                        $" - {forecast.weather?.FirstOrDefault()?.description?.ToString()}"
            };
    }
}