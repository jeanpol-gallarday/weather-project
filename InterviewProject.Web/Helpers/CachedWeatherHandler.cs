using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace InterviewProject.Helpers
{
    public class CachedWeatherHandler : DelegatingHandler
    {
        private readonly IMemoryCache _cache;

        public CachedWeatherHandler(IMemoryCache cache)
        {
            _cache = cache;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var queryString = HttpUtility.ParseQueryString(request.RequestUri!.Query);
            var query = queryString["q"];
            var units = queryString["units"];
            var key = $"{query}-{units}";

            var cachedResponse = await _cache.GetOrCreateAsync(key, async (cacheEntry) =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                var response = await base.SendAsync(request, cancellationToken);
                var content = await response.Content.ReadAsStringAsync();

                return new HttpResponseMessage(response.StatusCode)
                {
                    Content = new StringContent(content)
                };
            });

            return cachedResponse;
        }
    }
}