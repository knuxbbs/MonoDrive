using System.Threading.Tasks;

namespace MonoDrive.Application.Interfaces
{
    public interface IGoogleDriveAppService : IAppServiceBase
    {
        Task CreateFolders(string parentFolderPath);
        Task DownloadFolderStructure(string remoteFolderName, string parentFolderPath);
        Task DownloadFiles();
    }
}