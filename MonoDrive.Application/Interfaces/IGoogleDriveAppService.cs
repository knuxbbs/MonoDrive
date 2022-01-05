using System.Threading;
using System.Threading.Tasks;

namespace MonoDrive.Application.Interfaces
{
    public interface IGoogleDriveAppService : IAppServiceBase
    {
        Task DownloadAndCreateDirectories(string parentDirectoryPath, CancellationToken cancellationToken = default);
        Task DownloadFiles();
    }
}