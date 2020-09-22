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
        private readonly IGoogleScriptAppService _googleScriptAppService;
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;

        public ConsoleHostedService(IGoogleOAuthAppService oAuthAppService,
            IGoogleScriptAppService googleScriptAppService,
            ILogger<ConsoleHostedService> logger,
            IHostApplicationLifetime appLifetime)
        {
            _oAuthAppService = oAuthAppService;
            _googleScriptAppService = googleScriptAppService;
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
                        var googleUserInfo = await _oAuthAppService.GetUserInfo();
                        
                        Console.WriteLine("User credentials:");
                        Console.WriteLine($"\tId: {googleUserInfo.Id}");
                        Console.WriteLine($"\tEmail: {googleUserInfo.Email}");
                        Console.WriteLine($"\tName: {googleUserInfo.Name}");
                        Console.WriteLine($"\tETag: {googleUserInfo.ETag}");
                        Console.WriteLine($"\tGender: {googleUserInfo.Gender}");
                        Console.WriteLine($"\tPicture: {googleUserInfo.Picture}");

                        await _googleScriptAppService.GetFoldersUnderRoot();
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
                }, cancellationToken);
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}