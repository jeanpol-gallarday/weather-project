using InterviewProject.Controllers.Contracts;
using InterviewProject.Exceptions;
using InterviewProject.Helpers;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace InterviewProject.Services.HttpServices
{
    public class WeatherHttpService : ErrorHandling
    {
        private readonly HttpClient _client;
        private const string apikey = "eed848d51c3680219436999d8914344b";

        public WeatherHttpService(HttpClient client)
            : base(nameof(WeatherHttpService))
        {
            _client = client;
        }

        public async Task<WeatherForecastRoot?> GetWeatherForecastAsync(string location)
        {
            var response = await _client.GetAsync($"?q={location}&appid={apikey}&cnt=40&units=metric");

            await HandleResponseAsync(response);

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new ResourceNotFoundException(nameof(location), location);

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<WeatherForecastRoot>(jsonResponse);
        }
    }
}