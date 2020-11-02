using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MonoDrive.Infra.IoC;
using Xunit.Abstractions;

namespace MonoDrive.Application.Test
{
    public abstract class AppServiceBaseTest
    {
        protected AppServiceBaseTest(ITestOutputHelper outputHelper)
        {
            var host = GenericHost.CreateHostBuilder(null)
                .ConfigureLogging(builder => builder.AddXUnit(outputHelper))
                .Build();
            
            ServiceProvider = host.Services;
            OutputHelper = outputHelper;
        }

        protected IServiceProvider ServiceProvider { get; }
        protected ITestOutputHelper OutputHelper { get; }
    }
}