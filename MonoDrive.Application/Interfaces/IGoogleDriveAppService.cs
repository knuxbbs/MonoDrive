using System.Threading.Tasks;

namespace MonoDrive.Application.Interfaces
{
    public interface IGoogleDriveAppService : IAppServiceBase
    {
        void DownloadFolderStructure(string remoteFolderName, string parentFolderPath);
        Task DownloadFiles();
    }
}