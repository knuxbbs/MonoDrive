using ODrive.Sharp.Application.Interfaces;
using Google.Apis.Services;
using Google.Apis.Oauth2.v2;
using Google.Apis.Drive.v3;
using System.Threading.Tasks;

namespace ODrive.Sharp.Application.Services
{
    public class GoogleApisServiceProvider : IGoogleApisServiceProvider
    {
        private readonly IGoogleClientAuthProvider _googleClientAuthProvider;
        private BaseClientService.Initializer _initializer;

        public GoogleApisServiceProvider(IGoogleClientAuthProvider googleClientAuthProvider)
        {
            _googleClientAuthProvider = googleClientAuthProvider;
        }

        private async Task<BaseClientService.Initializer> GetBaseClientService()
        {
            _initializer = new BaseClientService.Initializer
            {
                HttpClientInitializer = await _googleClientAuthProvider.GetUserCredential(),
                ApplicationName = "knuxbbs Open Drive"
            };

            return _initializer;
        }

        public Oauth2Service Oauth2Service
        {
            get
            {
                return new Oauth2Service(_initializer);
            }
        }

        public DriveService DriveService
        {
            get
            {
                return new DriveService(_initializer);
            }
        }
    }
}