using System;
using System.Net.Http;
using ComposableAsync;
using Google.Apis.Http;
using Google.Apis.Services;
using Microsoft.Extensions.DependencyInjection;
using MonoDrive.Application.Interfaces;
using MonoDrive.Application.Presenters;
using MonoDrive.Application.Providers;
using MonoDrive.Application.Services;
using RateLimiter;

namespace MonoDrive.Infra.IoC
{
    public class NativeInjectorBootstrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IGoogleClientAuthProvider, GoogleClientAuthProvider>();
            services.AddSingleton<IGoogleApiServiceProvider, GoogleApiServiceProvider>();

            services.AddSingleton<IGoogleOAuthAppService, GoogleOAuthAppService>();
            services.AddSingleton<IGoogleDriveAppService, GoogleDriveAppService>();

            services.AddSingleton<IMainWindowPresenter, MainWindowPresenter>();

            services.AddHttpClient("google")
                .ConfigurePrimaryHttpMessageHandler(() => TimeLimiter
                    .GetFromMaxCountByInterval(60, TimeSpan.FromMinutes(1))
                    .AsDelegatingHandler());
        }

        private static BaseClientService.Initializer GetBaseClientService(
            IConfigurableHttpClientInitializer httpClientInitializer)
        {
            return new BaseClientService.Initializer
            {
                HttpClientInitializer = httpClientInitializer,
                //HttpClientFactory = 
                ApplicationName = "knuxbbs Open Drive"
            };
        }
    }
}