using Google.Apis.Http;
using Google.Apis.Services;
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
            services.AddSingleton<IGoogleClientAuthProvider, GoogleClientAuthProvider>();
            services.AddSingleton<IGoogleApiServiceProvider, GoogleApiServiceProvider>();

            services.AddSingleton<IGoogleOAuthAppService, GoogleOAuthAppService>();
            services.AddSingleton<IGoogleDriveAppService, GoogleDriveAppService>();

            services.AddSingleton<IMainWindowPresenter, MainWindowPresenter>();
        }

        private static BaseClientService.Initializer GetBaseClientService(
            IConfigurableHttpClientInitializer httpClientInitializer)
        {
            return new BaseClientService.Initializer
            {
                HttpClientInitializer = httpClientInitializer,
                ApplicationName = "knuxbbs Open Drive"
            };
        }
    }
}