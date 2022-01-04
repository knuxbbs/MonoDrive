using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MonoDrive.Cli.HostedServices;
using MonoDrive.Infra.IoC;

namespace MonoDrive.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await GenericHost.GetBuilder(args)
                .ConfigureServices(RegisterServices)
                .RunConsoleAsync();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddHostedService<ConsoleHostedService>();
        }
    }
}