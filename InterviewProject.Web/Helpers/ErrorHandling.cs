using InterviewProject.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace InterviewProject.Helpers
{
    public abstract class ErrorHandling
    {
        private readonly string _resourceName;

        protected ErrorHandling(string resourceName)
        {
            _resourceName = resourceName;
        }

        /// ---------------------------------------------------------------------------------------
        /// <summary>
        /// Handles the response.
        /// </summary>
        /// <param name="responseMessage">The operation response.</param>
        /// <exception cref="Exception">Resource Unavailable</exception>
        /// <exception cref="Exception">Forbidden</exception>
        /// <exception cref="Exception">BadRequest</exception>
        /// <exception cref="HttpRequestException">Something else</exception>
        /// ---------------------------------------------------------------------------------------
        protected async Task HandleResponseAsync(HttpResponseMessage responseMessage)
        {
            if (responseMessage is null)
                throw new ArgumentNullException(nameof(responseMessage));

            if (responseMessage.IsSuccessStatusCode)
                return;

            switch (responseMessage.StatusCode)
            {
                case HttpStatusCode.ServiceUnavailable:
                    throw new BusinessException($"{_resourceName} Service unavailable");

                case HttpStatusCode.NotFound:
                    break; // do nothing let the caller decide

                case HttpStatusCode.Forbidden:
                    throw new EntitySecurityViolationException($"{_resourceName} Message: {await GetMessage(responseMessage.Content)}");

                case HttpStatusCode.BadRequest:
                    throw new BadRequestException($"{_resourceName} returned BadRequest. Reason [{GetMessage(responseMessage.Content)}]");

                case HttpStatusCode.Conflict:
                    break;

                default:
                    throw new HttpRequestException($"{responseMessage.StatusCode}: {responseMessage.ReasonPhrase} returned by {_resourceName}");
            }
        }

        protected async Task<string> GetMessage(HttpContent content)
        {
            var contentString = await content.ReadAsStringAsync();
            try
            {
                // try to parse it
                var errorContent = JObject.Parse(contentString);
                var message = (string?)errorContent["message"];

                // if we can't get the message then return the content as-is
                return string.IsNullOrEmpty(message) ? contentString : message;
            }
            catch (JsonReaderException)
            {
                return contentString;
            }
        }
    }
}