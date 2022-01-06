using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MonoDrive.Infra.IoC;

namespace MonoDrive.Gtk
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            global::Gtk.Application.Init();
            
            var hostBuilder = GenericHost.GetBuilder(args);
            hostBuilder.ConfigureServices(RegisterServices);
            
            using var host = hostBuilder.Build();
            host.Start();

            var app = host.Services.GetService<Startup>();
            app.Run();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<Startup>();
        }
    }
}