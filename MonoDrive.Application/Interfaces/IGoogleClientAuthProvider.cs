using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;

namespace MonoDrive.Application.Interfaces
{
    public interface IGoogleClientAuthProvider
    {
        Task<UserCredential> GetUserCredential();
    }
}