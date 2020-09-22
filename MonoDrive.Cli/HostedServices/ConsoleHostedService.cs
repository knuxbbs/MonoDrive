using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MonoDrive.Application.Interfaces;

namespace MonoDrive.Cli.HostedServices
{
    public class ConsoleHostedService : IHostedService
    {
        private readonly IGoogleOAuthAppService _oAuthAppService;
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;

        public ConsoleHostedService(IGoogleOAuthAppService oAuthAppService,
            ILogger<ConsoleHostedService> logger,
            IHostApplicationLifetime appLifetime)
        {
            _oAuthAppService = oAuthAppService;
            _logger = logger;
            _appLifetime = appLifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Starting with arguments: {string.Join(" ", Environment.GetCommandLineArgs())}");

            _appLifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        _logger.LogInformation("Hello World!");

                        var googleUserInfo = await _oAuthAppService.GetUserInfo();
                        _logger.LogInformation($"Id: {googleUserInfo.Id}");
                        _logger.LogInformation($"Email: {googleUserInfo.Email}");
                        _logger.LogInformation($"Name: {googleUserInfo.Name}");
                        _logger.LogInformation($"ETag: {googleUserInfo.ETag}");
                        _logger.LogInformation($"Gender: {googleUserInfo.Gender}");
                        _logger.LogInformation($"Picture: {googleUserInfo.Picture}");
                        
                        // Simulate real work is being done
                        await Task.Delay(1000);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unhandled exception!");
                    }
                    finally
                    {
                        // Stop the application once the work is done
                        //_appLifetime.StopApplication();
                    }
                });
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}