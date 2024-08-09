using InterviewProject.DTOs;
using InterviewProject.Exceptions;
using InterviewProject.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InterviewProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        public IWeatherForecastService _weatherService;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(
            IWeatherForecastService weatherService,
            ILogger<WeatherForecastController> logger)
        {
            _weatherService = weatherService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeatherForecast>>> GetAsync([FromQuery] string location)
        {
            try
            {
                var weatherForecast = await _weatherService.GetWeatherForecastAsync(location);
                return Ok(weatherForecast);
            }
            catch (Exception ex)
            when (ex is InvalidArgumentException || ex is BadRequestException)
            {
                _logger.LogError($"An error ocurred while trying to call the service. {ex.Message}");
                return BadRequest(new ObjectResult(new { message = ex.Message }));
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogError($"Resource was not found. {ex.Message}");
                return NotFound(new ObjectResult(new { message = ex.Message }));
            }
        }
    }
}