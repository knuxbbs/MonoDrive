using Google.Apis.Oauth2.v2;
using Google.Apis.Drive.v3;

namespace ODrive.Sharp.Application.Interfaces
{
    public interface IGoogleApisServiceProvider
    {
        Oauth2Service Oauth2Service { get; }
        DriveService DriveService { get; }
    }
}