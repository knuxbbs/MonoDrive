using System;
using System.Net.Http;
using ComposableAsync;
using Google.Apis.Http;
using RateLimiter;

namespace MonoDrive.Infra.IoC
{
    public class GoogleHttpClientFactory : HttpClientFactory
    {
        protected override HttpMessageHandler CreateHandler(CreateHttpClientArgs args)
        {
            var handler = TimeLimiter
                .GetFromMaxCountByInterval(1000, TimeSpan.FromSeconds(100))
                .AsDelegatingHandler();
            
            handler.InnerHandler = base.CreateHandler(args);
            
            return handler;
        }
    }
}