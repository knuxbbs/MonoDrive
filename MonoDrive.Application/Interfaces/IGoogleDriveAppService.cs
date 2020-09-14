using System.Threading.Tasks;

namespace ODrive.Sharp.Application.Interfaces
{
    public interface IGoogleDriveAppService : IAppServiceBase
    {
        Task DownloadFolderStructure(string remoteFolderName, string parentFolderPath);
        Task DownloadFiles();
    }
}