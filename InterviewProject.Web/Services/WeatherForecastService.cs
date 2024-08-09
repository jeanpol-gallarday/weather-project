using InterviewProject.Controllers.Contracts;
using InterviewProject.DTOs;
using InterviewProject.Exceptions;
using InterviewProject.Mappers.Interfaces;
using InterviewProject.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InterviewProject.Services
{
    public class WeatherForecastService : IWeatherForecastService
    {
        private const string defaultLocation = "Lima";
        private readonly IWeatherForecastMapper _weatherForecastMapper;
        private readonly IWeatherHttpService _weatherHttpService;
        private readonly ILogger<WeatherForecastService> _logger;

        public WeatherForecastService(
            IWeatherForecastMapper weatherForecastMapper,
            IWeatherHttpService weatherHttpService,
            ILogger<WeatherForecastService> logger)
        {
            _weatherForecastMapper = weatherForecastMapper;
            _weatherHttpService = weatherHttpService;
            _logger = logger;
        }
        public async Task<List<WeatherForecast>> GetWeatherForecastAsync(string location)
        {
            try
            {
                location = location ?? defaultLocation;
                if (!isValidLocation(location))
                    throw new InvalidArgumentException($"Invalid parameter: {nameof(location)}.");

                var weatherForecastRoot = await _weatherHttpService.GetWeatherForecastAsync(location.Trim());

                var fiveDayForecastList = weatherForecastRoot != null
                                            ? getFiveDayForecast(weatherForecastRoot)
                                            : Enumerable.Empty<forecast>().ToList();

                var weatherForecastDTO = _weatherForecastMapper.Map(fiveDayForecastList);

                return weatherForecastDTO;
            }
            catch (InvalidArgumentException)
            {
                _logger.LogError($"Invalid parameter 'location'. Make sure that this is a valid location.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error ocurred when trying to call the weather service: {ex.Message}");
                throw;
            }

        }

        private List<forecast> getFiveDayForecast(WeatherForecastRoot weatherForecastRoot)
        {
            var periodsPerDay = weatherForecastRoot.cnt / 5;
            var forecastList = weatherForecastRoot.list.OrderBy(x => x.dt_txt).ToList();
            var fiveDayForecastList = new List<forecast>();

            for (int i = 0; i < forecastList.Count; i += periodsPerDay)
            {
                fiveDayForecastList.Add(forecastList[i]);
            }

            return fiveDayForecastList;
        }

        // Check if the input is only letters and has at most 50 characters
        private static bool isValidLocation(string location)
            => location.Length <= 50 && Regex.IsMatch(location, "^[A-Za-z ]+$");
    }
}