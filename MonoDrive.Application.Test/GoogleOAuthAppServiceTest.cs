using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MonoDrive.Application.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace MonoDrive.Application.Test
{
    public class GoogleOAuthAppServiceTest : AppServiceBaseTest
    {
        private readonly IGoogleOAuthAppService _oAuthAppService;

        public GoogleOAuthAppServiceTest(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            _oAuthAppService = ServiceProvider.GetService<IGoogleOAuthAppService>();
        }

        [Fact]
        public async Task GetUserInfo()
        {
            var googleUserInfo = await _oAuthAppService.GetUserInfo();
            
            Assert.NotNull(googleUserInfo.Id);
            Assert.NotNull(googleUserInfo.Email);
            Assert.NotNull(googleUserInfo.Picture);
            
            OutputHelper.WriteLine("User credentials:");
            OutputHelper.WriteLine($"\tId: {googleUserInfo.Id}");
            OutputHelper.WriteLine($"\tEmail: {googleUserInfo.Email}");
            OutputHelper.WriteLine($"\tName: {googleUserInfo.Name}");
            OutputHelper.WriteLine($"\tETag: {googleUserInfo.ETag}");
            OutputHelper.WriteLine($"\tGender: {googleUserInfo.Gender}");
            OutputHelper.WriteLine($"\tPicture: {googleUserInfo.Picture}");
        }
    }
}