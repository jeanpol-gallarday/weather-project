using AutoFixture;
using InterviewProject.Controllers.Contracts;
using InterviewProject.DTOs;
using InterviewProject.Exceptions;
using InterviewProject.Mappers.Interfaces;
using InterviewProject.Services;
using InterviewProject.Services.Interfaces;
using InterviewProject.Tests.Base;
using Microsoft.Extensions.Logging;
using Moq;

namespace InterviewProject.Tests.Services
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class WeatherForecastServiceTests : BaseTests
    {
        private IFixture _fixture = new Fixture();
        private class WeatherForecastServiceBuilder
        {
            private Mock<IWeatherForecastMapper> _weatherForecastMapperMock;
            private Mock<ILogger<WeatherForecastService>> _loggerMock;
            private Mock<IWeatherHttpService> _weatherHttpServiceMock;
            public Mock<IWeatherForecastMapper> WeatherForecastMapperMock => _weatherForecastMapperMock ??= new Mock<IWeatherForecastMapper>();
            public Mock<ILogger<WeatherForecastService>> LoggerMock => _loggerMock ??= new Mock<ILogger<WeatherForecastService>>();
            public Mock<IWeatherHttpService> WeatherHttpServiceMock => _weatherHttpServiceMock ??= new Mock<IWeatherHttpService>();

            public WeatherForecastService Build()
            {
                return new WeatherForecastService(WeatherForecastMapperMock.Object, WeatherHttpServiceMock.Object, LoggerMock.Object);
            }

        }

        [TestCase("ValidLocation")]
        [TestCase("Lima")]
        [TestCase("New York")]
        public async Task GetWeatherForecastAsync_ReturnsWeatherForecasts_WhenLocationIsValid(string location)
        {
            // Arrange
            var resourceBuilder = new WeatherForecastServiceBuilder();
            var resource = resourceBuilder.Build();

            var weatherForecastRoot = _fixture
                .Build<WeatherForecastRoot>()
                .With(w => w.cnt, 40)
                .With(w => w.list, _fixture.CreateMany<forecast>(40).ToList())
                .Create();

            var expectedForecasts = _fixture.CreateMany<WeatherForecast>(5).ToList();

            resourceBuilder
                .WeatherHttpServiceMock
                .Setup(s => s.GetWeatherForecastAsync(location))
                .ReturnsAsync(weatherForecastRoot);

            resourceBuilder
                .WeatherForecastMapperMock
                .Setup(m => m.Map(It.IsAny<List<forecast>>()))
                .Returns(expectedForecasts);

            // Act
            var result = await resource.GetWeatherForecastAsync(location);

            // Assert
            Assert.That(result, Is.EqualTo(expectedForecasts));
        }

        [TestCase("InvalidLocation123")]
        [TestCase("12345")]
        [TestCase("Location$_@")]
        public void GetWeatherForecastAsync_ThrowsInvalidArgumentException_WhenLocationIsInvalid(string location)
        {
            // Arrange
            var resourceBuilder = new WeatherForecastServiceBuilder();
            var resource = resourceBuilder.Build();
            var expectedErrorMessage = $"Invalid parameter: {nameof(location)}.";

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidArgumentException>(() => resource.GetWeatherForecastAsync(location));
            Assert.That(ex.Message, Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void GetWeatherForecastAsync_ThrowsInvalidArgumentException_WhenLocationIsGreaterThan50Characters()
        {
            // Arrange
            var resourceBuilder = new WeatherForecastServiceBuilder();
            var resource = resourceBuilder.Build();
            var location = new string('A', 51);
            var expectedErrorMessage = $"Invalid parameter: {nameof(location)}.";

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidArgumentException>(() => resource.GetWeatherForecastAsync(location));
            Assert.That(ex.Message, Is.EqualTo(expectedErrorMessage));
        }

        [Test]
        public void GetWeatherForecastAsync_ThrowsException_AndLogsError_WhenExceptionIsThrown()
        {
            // Arrange
            var resourceBuilder = new WeatherForecastServiceBuilder();
            var resource = resourceBuilder.Build();
            var location = "ValidLocation";
            var exceptionMessage = "Some error occurred";
            var expectedErrorMessage = "An error ocurred when trying to call the weather service";

            resourceBuilder
                .WeatherHttpServiceMock
                .Setup(s => s.GetWeatherForecastAsync(location))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => resource.GetWeatherForecastAsync(location));
            Assert.That(ex.Message, Is.EqualTo(exceptionMessage));
            IsExpectedLog(resourceBuilder.LoggerMock, expectedErrorMessage, Times.Once, LogLevel.Error);
        }

        [Test]
        public async Task GetWeatherForecastAsync_ReturnsDefaultLocation_WhenLocationIsNull()
        {
            // Arrange
            var resourceBuilder = new WeatherForecastServiceBuilder();
            var resource = resourceBuilder.Build();
            string? location = null;
            var defaultLocation = "Lima";
            var weatherForecastRoot = _fixture.Build<WeatherForecastRoot>()
                .With(w => w.cnt, 40)
                .With(w => w.list, _fixture.CreateMany<forecast>(40).ToList())
                .Create();

            var expectedForecasts = _fixture.CreateMany<WeatherForecast>(5).ToList();

            resourceBuilder
                .WeatherHttpServiceMock
                .Setup(s => s.GetWeatherForecastAsync(defaultLocation))
                .ReturnsAsync(weatherForecastRoot);

            resourceBuilder
                .WeatherForecastMapperMock
                .Setup(m => m.Map(It.IsAny<List<forecast>>()))
                .Returns(expectedForecasts);

            // Act
            var result = await resource.GetWeatherForecastAsync(location);

            // Assert
            Assert.That(result, Is.EqualTo(expectedForecasts));
        }

        [Test]
        public async Task GetWeatherForecastAsync_ReturnsEmptyList_WhenWeatherForecastRootIsNull()
        {
            // Arrange
            var resourceBuilder = new WeatherForecastServiceBuilder();
            var resource = resourceBuilder.Build();
            var location = "Lima";
            WeatherForecastRoot? weatherForecastRoot = null;
            var expectedForecasts = new List<WeatherForecast>();

            resourceBuilder
                .WeatherHttpServiceMock
                .Setup(s => s.GetWeatherForecastAsync(location))
                .ReturnsAsync(weatherForecastRoot);

            resourceBuilder
                .WeatherForecastMapperMock
                .Setup(m => m.Map(It.IsAny<List<forecast>>()))
                .Returns(expectedForecasts);

            // Act
            var result = await resource.GetWeatherForecastAsync(location);

            // Assert
            Assert.That(result, Is.EqualTo(expectedForecasts));
        }
    }
}