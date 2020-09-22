using Google.Apis.Drive.v3;
using Google.Apis.Http;
using Google.Apis.Oauth2.v2;
using Google.Apis.Script.v1;
using Google.Apis.Services;
using MonoDrive.Application.Interfaces;

namespace MonoDrive.Application.Providers
{
    public class GoogleApiServiceProvider : IGoogleApiServiceProvider
    {
        private readonly BaseClientService.Initializer _initializer;

        public GoogleApiServiceProvider(IConfigurableHttpClientInitializer httpClientInitializer,
            IHttpClientFactory httpClientFactory)
        {
            _initializer = new BaseClientService.Initializer
            {
                HttpClientInitializer = httpClientInitializer,
                HttpClientFactory = httpClientFactory,
                ApplicationName = "MonoDrive"
            };
        }

        public DriveService GetDriveService()
        {
            return new DriveService(_initializer);
        }

        public Oauth2Service GetOauth2Service()
        {
            return new Oauth2Service(_initializer);
        }
        
        public ScriptService GetScriptService()
        {
            return new ScriptService(_initializer);
        }
    }
}