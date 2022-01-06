using Google.Apis.Drive.v3;
using Google.Apis.Oauth2.v2;

namespace MonoDrive.Application.Interfaces
{
    public interface IGoogleApiServiceProvider
    {
        DriveService GetDriveService();
        Oauth2Service GetOauth2Service();
    }
}