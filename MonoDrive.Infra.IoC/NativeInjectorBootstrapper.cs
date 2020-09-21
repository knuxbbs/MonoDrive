using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Http;
using Google.Apis.Oauth2.v2;
using Google.Apis.Util.Store;
using Microsoft.Extensions.DependencyInjection;
using MonoDrive.Application.Interfaces;
using MonoDrive.Application.Presenters;
using MonoDrive.Application.Providers;
using MonoDrive.Application.Services;

namespace MonoDrive.Infra.IoC
{
    public class NativeInjectorBootstrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IGoogleApiServiceProvider, GoogleApiServiceProvider>();

            services.AddSingleton<IGoogleOAuthAppService, GoogleOAuthAppService>();
            services.AddSingleton<IGoogleDriveAppService, GoogleDriveAppService>();

            services.AddSingleton<IConfigurableHttpClientInitializer>(x => GetUserCredential().Result);
            services.AddSingleton<IHttpClientFactory, CustomHttpClientFactory>();

            // services.AddHttpClient("google")
            //     .ConfigurePrimaryHttpMessageHandler(() => TimeLimiter
            //         .GetFromMaxCountByInterval(60, TimeSpan.FromMinutes(1))
            //         .AsDelegatingHandler());

            services.AddSingleton<IMainWindowPresenter, MainWindowPresenter>();
        }

        private static Task<UserCredential> GetUserCredential()
        {
            string[] scopes =
            {
                Oauth2Service.Scope.UserinfoEmail,
                DriveService.Scope.DriveReadonly
            };

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    new FileDataStore("token.json", true));
            }
        }
    }
}