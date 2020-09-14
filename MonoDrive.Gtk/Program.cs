using System;
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
            
            var host = CreateHostBuilder(args).Build();
            host.Start();

            var app = host.Services.GetService<Startup>();
            app.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
             .ConfigureServices((hostContext, services) =>
                {
                    NativeInjectorBootstrapper.RegisterServices(services);

                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<Startup>();
                })
                .UseConsoleLifetime();
    }
}