using System.Threading.Tasks;
using MonoDrive.Application.Models;

namespace MonoDrive.Application.Interfaces
{
    public interface IGoogleOAuthAppService : IAppServiceBase
    {
        Task<GoogleUserInfo> GetUserInfo();
    }
}