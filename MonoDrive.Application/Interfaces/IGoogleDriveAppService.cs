using System.Threading.Tasks;

namespace MonoDrive.Application.Interfaces
{
    public interface IGoogleDriveAppService : IAppServiceBase
    {
        Task DownloadAndCreateFolders(string parentDirectoryPath);
        Task DownloadFiles();
    }
}