using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Http;
using Google.Apis.Oauth2.v2;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MonoDrive.Application.Interfaces;
using MonoDrive.Application.Presenters;
using MonoDrive.Application.Providers;
using MonoDrive.Application.Services;

namespace MonoDrive.Infra.IoC
{
    public class NativeInjectorBootstrapper
    {
        public static void RegisterServices(HostBuilderContext builderContext, IServiceCollection services)
        {
            services.AddSingleton<IGoogleApiServiceProvider, GoogleApiServiceProvider>();

            services.AddSingleton<IGoogleOAuthAppService, GoogleOAuthAppService>();
            services.AddSingleton<IGoogleDriveAppService, GoogleDriveAppService>();
            services.AddSingleton<IGoogleScriptAppService, GoogleScriptAppService>();

            var googleClientCredentials = builderContext.Configuration.GetSection("installed");

            services.AddSingleton<IConfigurableHttpClientInitializer>(x =>
                GetUserCredential(googleClientCredentials).Result);
            services.AddSingleton<IHttpClientFactory, GoogleHttpClientFactory>();

            // services.AddHttpClient("google")
            //     .ConfigurePrimaryHttpMessageHandler(() => TimeLimiter
            //         .GetFromMaxCountByInterval(60, TimeSpan.FromMinutes(1))
            //         .AsDelegatingHandler());

            services.AddSingleton<IMainWindowPresenter, MainWindowPresenter>();
        }

        private static Task<UserCredential> GetUserCredential(IConfiguration configuration)
        {
            return GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = configuration["client_id"],
                    ClientSecret = configuration["client_secret"]
                },
                new []
                {
                    Oauth2Service.Scope.UserinfoEmail,
                    DriveService.Scope.DriveReadonly
                },
                "user",
                CancellationToken.None,
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                new FileDataStore("token.json", true));
        }
    }
}