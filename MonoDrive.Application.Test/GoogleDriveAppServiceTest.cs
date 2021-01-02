using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MonoDrive.Application.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace MonoDrive.Application.Test
{
    public class GoogleDriveAppServiceTest : AppServiceBaseTest
    {
        private readonly IGoogleDriveAppService _driveAppService;

        public GoogleDriveAppServiceTest(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            _driveAppService = ServiceProvider.GetService<IGoogleDriveAppService>();
        }

        [Fact]
        public async Task DownloadAndCreateFolders()
        {
            await _driveAppService.DownloadAndCreateFolders("MonoDrive Test");
        }
    }
}