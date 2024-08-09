using Polly;
using Polly.Contrib.WaitAndRetry;
using System;
using System.Net;
using System.Net.Http;

namespace InterviewProject.Extensions
{
    public class PollyPolicyExtensions
    {
        public static IAsyncPolicy<HttpResponseMessage> GetRetryAfterPolicy()
        {
            var retryPolicy =
                Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(x => x.StatusCode is >= HttpStatusCode.InternalServerError or HttpStatusCode.RequestTimeout)
                .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5));

            return retryPolicy;
        }
    }
}