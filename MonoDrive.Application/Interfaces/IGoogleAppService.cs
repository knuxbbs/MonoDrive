using System.Threading.Tasks;
using Google.Apis.Drive.v3;
using Google.Apis.Oauth2.v2;

namespace MonoDrive.Application.Interfaces
{
    public interface IGoogleApiServiceProvider
    {
        Task<Oauth2Service> GetOauth2Service();
        Task<DriveService> GetDriveService();
    }
}