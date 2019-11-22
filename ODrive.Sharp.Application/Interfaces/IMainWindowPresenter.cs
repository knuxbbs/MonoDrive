using System.Threading.Tasks;

namespace ODrive.Sharp.Application.Interfaces
{
    public interface IMainWindowPresenter
    {
        Task<string> GetUserEmail();
        Task Sync(string folderPath);
    }
}