using System.Threading.Tasks;

namespace MonoDrive.Application.Interfaces
{
    public interface IMainWindowPresenter
    {
        Task<string> GetUserEmail();
        Task Sync(string folderPath);
    }
}