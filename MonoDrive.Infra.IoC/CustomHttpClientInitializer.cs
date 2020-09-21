using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Http;
using Google.Apis.Oauth2.v2;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;

namespace MonoDrive.Infra.IoC
{
    public class CustomHttpClientInitializer : IConfigurableHttpClientInitializer
    {
        private readonly IConfiguration _configuration;

        public CustomHttpClientInitializer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Initialize(ConfigurableHttpClient httpClient)
        {
            throw new System.NotImplementedException();
        }
        
        private static readonly string[] Scopes =
        {
            Oauth2Service.Scope.UserinfoEmail,
            DriveService.Scope.DriveReadonly
        };
        
        public async Task<UserCredential> GetUserCredential()
        {
            //See: https://stackoverflow.com/questions/8047064/convert-string-to-system-io-stream
            //byte[] byteArray = Encoding.UTF8.GetBytes(_configuration.GetSection("installed").Value);
            
            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                try
                {
                    return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        // The file token.json stores the user's access and refresh tokens, and is created
                        // automatically when the authorization flow completes for the first time.
                        new FileDataStore("token.json", true)).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}