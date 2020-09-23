using System.Threading.Tasks;

namespace MonoDrive.Application.Interfaces
{
    public interface IGoogleDriveAppService : IAppServiceBase
    {
        Task GetFoldersStructure();
        Task DownloadFolderStructure(string remoteFolderName, string parentFolderPath);
        Task DownloadFiles();
    }
}