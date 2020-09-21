using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MonoDrive.Infra.IoC
{
    public class GenericHost
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .ConfigureServices((builderContext, services) =>
                {
                    NativeInjectorBootstrapper.RegisterServices(services);
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    //logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                })
                .UseConsoleLifetime();
    }
}