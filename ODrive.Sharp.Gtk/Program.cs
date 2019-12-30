using System;
using System.Linq;
using GLib;
using Gtk;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ODrive.Sharp.Infra.IoC;

namespace ODrive.Sharp.Gtk
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
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