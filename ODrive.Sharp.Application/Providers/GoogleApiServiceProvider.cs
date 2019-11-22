using System;
using System.Threading.Tasks;
using Google.Apis.Drive.v3;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;
using ODrive.Sharp.Application.Interfaces;

namespace ODrive.Sharp.Application.Providers
{
    public class GoogleApiServiceProvider : IGoogleApiServiceProvider
    {
        private readonly Lazy<Task<BaseClientService.Initializer>> _initializer;

        public GoogleApiServiceProvider(IGoogleClientAuthProvider googleClientAuthProvider)
        {
            _initializer = new Lazy<Task<BaseClientService.Initializer>>(async () =>
                new BaseClientService.Initializer
                {
                    HttpClientInitializer = await googleClientAuthProvider.GetUserCredential(),
                    ApplicationName = "knuxbbs Open Drive"
                });
        }

        public async Task<Oauth2Service> GetOauth2Service()
        {
            return new Oauth2Service(await _initializer.Value);
        }
        
        public async Task<DriveService> GetDriveService()
        {
            return new DriveService(await _initializer.Value);
        }

        public void Dispose()
        {
            if (_initializer.IsValueCreated)
            {
                _initializer.Value.Dispose();
            }
        }
    }
}