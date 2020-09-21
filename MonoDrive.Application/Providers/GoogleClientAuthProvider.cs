using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Oauth2.v2;
using Google.Apis.Util.Store;
using MonoDrive.Application.Interfaces;

namespace MonoDrive.Application.Providers
{
    public class GoogleClientAuthProvider : IGoogleClientAuthProvider
    {
        private static readonly string[] Scopes =
        {
            Oauth2Service.Scope.UserinfoEmail,
            DriveService.Scope.DriveReadonly
        };

        public Task<UserCredential> GetUserCredential()
        {
            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    new FileDataStore("token.json", true));
            }
        }
    }
}