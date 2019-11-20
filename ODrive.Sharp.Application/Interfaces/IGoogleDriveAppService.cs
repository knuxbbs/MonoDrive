using System.Threading.Tasks;

namespace ODrive.Sharp.Application.Interfaces
{
    public interface IGoogleDriveAppService : IAppServiceBase
    {
        Task Sync(string folderPath);
        Task SignIn();
    }
}