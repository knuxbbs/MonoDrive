using System.Threading.Tasks;

namespace MonoDrive.Application.Interfaces
{
    public interface IGoogleScriptAppService
    {
        Task GetFoldersUnderRoot();
    }
}