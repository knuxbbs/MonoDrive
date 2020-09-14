using System.Threading.Tasks;

namespace MonoDrive.Application.Interfaces
{
    public interface IGoogleOAuthAppService : IAppServiceBase
    {
        Task<GoogleUserInfo> GetUserInfo();
    }
}