using System.Threading.Tasks;
using Google.Apis.Oauth2.v2;
using Google.Apis.Drive.v3;

namespace ODrive.Sharp.Application.Interfaces
{
    public interface IGoogleApiServiceProvider
    {
        Task<Oauth2Service> GetOauth2Service();
        Task<DriveService> GetDriveService();
    }
}