using System.Threading.Tasks;

namespace ODrive.Sharp.Application.Interfaces
{
    public interface IGoogleOAuthAppService : IAppServiceBase
    {
        Task<GoogleUserInfo> GetUserInfo();
    }
}