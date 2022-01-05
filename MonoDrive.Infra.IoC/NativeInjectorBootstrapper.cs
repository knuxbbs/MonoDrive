using System;
using System.Threading;
using System.Threading.Tasks;
using ComposableAsync;
using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Http;
using Google.Apis.Logging;
using Google.Apis.Oauth2.v2;
using LiteDB.Async;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MonoDrive.Application.Data;
using MonoDrive.Application.Interfaces;
using MonoDrive.Application.Presenters;
using MonoDrive.Application.Providers;
using MonoDrive.Application.Services;
using RateLimiter;

namespace MonoDrive.Infra.IoC
{
    public static class NativeInjectorBootstrapper
    {
        public static void RegisterServices(HostBuilderContext builderContext, IServiceCollection services)
        {
            services.AddSingleton<ILiteDatabaseAsync>(x =>
                new LiteDatabaseAsync(LiteDbHelper.GetFilePath(@"MonoDrive.db")));

            var googleClientCredentials = builderContext.Configuration.GetSection("installed");

            services.AddSingleton<IConfigurableHttpClientInitializer>(x =>
                GetUserCredential(googleClientCredentials, x.GetService<ILiteDatabaseAsync>()).Result);
            services.AddSingleton<IHttpClientFactory>(GoogleHttpClientFactory.CreateHttpClientFromMessageHandler);

            //Reference: https://github.com/googleapis/google-api-dotnet-client/blob/master/Src/Support/IntegrationTests/HttpClientFromMessageHandlerFactoryTests.cs
            services.AddHttpClient("TimeLimiter")
                .ConfigurePrimaryHttpMessageHandler(() => TimeLimiter
                    .GetFromMaxCountByInterval(1000, TimeSpan.FromSeconds(100))
                    .AsDelegatingHandler());

            services.AddSingleton<IGoogleApiServiceProvider, GoogleApiServiceProvider>();

            services.AddSingleton<IGoogleOAuthAppService, GoogleOAuthAppService>();
            services.AddSingleton<IGoogleDriveAppService, GoogleDriveAppService>();
            services.AddSingleton<IGoogleScriptAppService, GoogleScriptAppService>();

            services.AddSingleton<IMainWindowPresenter, MainWindowPresenter>();

            ApplicationContext.RegisterLogger(new ConsoleLogger(LogLevel.Warning));
        }

        private static Task<UserCredential> GetUserCredential(IConfiguration configuration,
            ILiteDatabaseAsync liteDatabaseAsync)
        {
            return GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = configuration["client_id"],
                    ClientSecret = configuration["client_secret"]
                },
                new[]
                {
                    Oauth2Service.Scope.UserinfoEmail,
                    DriveService.Scope.DriveReadonly
                },
                "user",
                CancellationToken.None,
                new LiteDbDataStore(liteDatabaseAsync));
        }
    }
}