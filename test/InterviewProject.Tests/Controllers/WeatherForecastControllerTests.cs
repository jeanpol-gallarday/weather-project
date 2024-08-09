using InterviewProject.Controllers;
using InterviewProject.DTOs;
using InterviewProject.Exceptions;
using InterviewProject.Services.Interfaces;
using InterviewProject.Tests.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace InterviewProject.Tests.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class WeatherForecastControllerTests : BaseTests
    {
        private WeatherForecastController GetController(WeatherForecastControllerAggregator mockAggregator)
            => new WeatherForecastController(mockAggregator.MockWeatherService.Object, mockAggregator.MockLogger.Object);

        public class WeatherForecastControllerAggregator
        {
            public Mock<IWeatherForecastService> MockWeatherService { get; } = new Mock<IWeatherForecastService>();
            public Mock<ILogger<WeatherForecastController>> MockLogger { get; } = new Mock<ILogger<WeatherForecastController>>();
        }

        [Test]
        public async Task GetAsync_ReturnsOkResult_WithWeatherForecast()
        {
            // Arrange
            var mockAggregator = new WeatherForecastControllerAggregator();
            var location = "TestLocation";
            var expectedResponseCode = 200;
            var weatherForecast = new List<WeatherForecast> { new WeatherForecast() };

            mockAggregator
                .MockWeatherService
                    .Setup(s => s.GetWeatherForecastAsync(location))
                    .ReturnsAsync(weatherForecast);

            var controller = GetController(mockAggregator);

            // Act
            var result = await controller.GetAsync(location);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.That(expectedResponseCode, Is.EqualTo(okResult.StatusCode));
            Assert.That(weatherForecast, Is.EqualTo(okResult.Value));
        }

        [Test]
        public async Task GetAsync_ReturnsBadRequest_OnInvalidArgumentException()
        {
            // Arrange
            var mockAggregator = new WeatherForecastControllerAggregator();
            var location = "TestLocation";
            var expectedResponseCode = 400;
            var expectedErrorMessage = "An error ocurred while trying to call the service";

            mockAggregator
                .MockWeatherService
                .Setup(s => s.GetWeatherForecastAsync(location))
                .ThrowsAsync(new InvalidArgumentException("Invalid argument"));

            var controller = GetController(mockAggregator);

            // Act
            var result = await controller.GetAsync(location);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.That(expectedResponseCode, Is.EqualTo(badRequestResult.StatusCode));
            IsExpectedLog(mockAggregator.MockLogger, expectedErrorMessage, Times.Once, LogLevel.Error);
        }

        [Test]
        public async Task GetAsync_ReturnsBadRequest_OnBadRequestException()
        {
            // Arrange
            var mockAggregator = new WeatherForecastControllerAggregator();
            var location = "TestLocation";
            var expectedResponseCode = 400;
            var expectedErrorMessage = "An error ocurred while trying to call the service";

            mockAggregator
                .MockWeatherService
                .Setup(s => s.GetWeatherForecastAsync(location))
                .ThrowsAsync(new BadRequestException("Bad request"));

            var controller = GetController(mockAggregator);

            // Act
            var result = await controller.GetAsync(location);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.That(expectedResponseCode, Is.EqualTo(badRequestResult.StatusCode));
            IsExpectedLog(mockAggregator.MockLogger, expectedErrorMessage, Times.Once, LogLevel.Error);
        }

        [Test]
        public async Task GetAsync_ReturnsNotFound_OnResourceNotFoundException()
        {
            // Arrange
            var mockAggregator = new WeatherForecastControllerAggregator();
            var location = "TestLocation";
            var expectedResponseCode = 404;
            var expectedErrorMessage = "Resource was not found";

            mockAggregator
                .MockWeatherService
                .Setup(s => s.GetWeatherForecastAsync(location))
                .ThrowsAsync(new ResourceNotFoundException("Resource not found"));

            var controller = GetController(mockAggregator);

            // Act
            var result = await controller.GetAsync(location);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.That(expectedResponseCode, Is.EqualTo(notFoundResult.StatusCode));
            IsExpectedLog(mockAggregator.MockLogger, expectedErrorMessage, Times.Once, LogLevel.Error);
        }
    }
}