using System.Threading.Tasks;

namespace MonoDrive.Application.Interfaces
{
    public interface IMainWindowPresenter
    {
        Task<string> GetUserEmail();
        Task<string> GetLocalRootDirectory();
        Task Sync(string folderPath);
    }
}