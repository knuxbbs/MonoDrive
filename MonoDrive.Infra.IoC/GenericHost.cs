using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MonoDrive.Infra.IoC
{
    public class GenericHost
    {
        public static IHostBuilder GetBuilder(string[] args) =>
            new HostBuilder()
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    config.AddJsonFile("credentials.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices(NativeInjectorBootstrapper.RegisterServices)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    //logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                })
                .UseConsoleLifetime();
    }
}