using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;

namespace ODrive.Sharp.Application.Interfaces
{
    public interface IGoogleClientAuthProvider
    {
        Task<UserCredential> GetUserCredential();
    }
}