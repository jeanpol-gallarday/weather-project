using AutoFixture;
using InterviewProject.Controllers.Contracts;
using InterviewProject.Mappers;

namespace InterviewProject.Tests.Mappers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class WeatherForecastMapperTests
    {
        private IFixture _fixture;
        private WeatherForecastMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _mapper = new WeatherForecastMapper();
        }

        [Test]
        public void Map_ReturnsWeatherForecastList_WhenInputIsValid()
        {
            // Arrange
            var forecasts = _fixture.CreateMany<forecast>(5).ToList();

            // Act
            var result = _mapper.Map(forecasts);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(forecasts.Count));

            for (int i = 0; i < forecasts.Count; i++)
            {
                Assert.That(result[i].Date, Is.EqualTo(forecasts[i].dt_txt));
                Assert.That(result[i].TemperatureC, Is.EqualTo(Convert.ToInt32(forecasts[i].main.temp)));

                var expectedSummary = $"{forecasts[i].weather.FirstOrDefault()?.main?.ToString()}" +
                                      $" - {forecasts[i].weather.FirstOrDefault()?.description?.ToString()}";

                Assert.That(result[i].Summary, Is.EqualTo(expectedSummary));
            }
        }

        [Test]
        public void Map_ReturnsEmptyList_WhenInputListIsEmpty()
        {
            // Arrange
            var emptyList = new List<forecast>();

            // Act
            var result = _mapper.Map(emptyList);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Map_HandlesNullWeatherDataGracefully()
        {
            // Arrange
            var forecasts = _fixture.CreateMany<forecast>(3).ToList();
            foreach (var forecast in forecasts)
            {
                forecast.weather = null;
            }

            // Act
            var result = _mapper.Map(forecasts);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(forecasts.Count));

            foreach (var weatherForecast in result)
            {
                Assert.That(weatherForecast.Summary, Is.EqualTo(" - "));
            }
        }

        [Test]
        public void Map_ReturnsCorrectMapping_WhenForecastListContainsEmptyWeatherEntries()
        {
            // Arrange
            var forecasts = _fixture.CreateMany<forecast>(5).ToList();
            forecasts[2].weather.Clear();

            // Act
            var result = _mapper.Map(forecasts);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(forecasts.Count));
            Assert.That(result[2].Summary, Is.EqualTo(" - "));
        }
    }
}
