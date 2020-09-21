using System;
using System.Net.Http;
using ComposableAsync;
using Google.Apis.Http;
using RateLimiter;

namespace MonoDrive.Infra.IoC
{
    public class CustomHttpClientFactory : HttpClientFactory
    {
        protected override HttpMessageHandler CreateHandler(CreateHttpClientArgs args)
        {
            var handler = TimeLimiter
                .GetFromMaxCountByInterval(60, TimeSpan.FromMinutes(1))
                .AsDelegatingHandler();

            handler.InnerHandler = base.CreateHandler(args);

            return handler;
        }
    }
}