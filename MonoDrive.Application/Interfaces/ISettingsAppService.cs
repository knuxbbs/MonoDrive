using System.Threading.Tasks;
using MonoDrive.Application.Models;

namespace MonoDrive.Application.Interfaces;

public interface ISettingsAppService : IAppServiceBase
{
    Task<AppSettings> GetAppSettings();
    Task UpdateLocalRootDirectory(string localRootDirectory);
}