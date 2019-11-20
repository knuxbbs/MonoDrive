using System;
using Microsoft.Extensions.DependencyInjection;
using ODrive.Sharp.Application.Interfaces;
using ODrive.Sharp.Application.Services;

namespace ODrive.Sharp.Infra.IoC
{
    public class NativeInjectorBootstrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IGoogleDriveAppService, GoogleDriveAppService>();
        }
    }
}