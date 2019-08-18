using System;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace ODrive.Sharp
{
    public class GoogleDrive
    {
        // The file token.json stores the user's access and refresh tokens, and is created
        // automatically when the authorization flow completes for the first time.
        private static readonly string[] Scopes = {DriveService.Scope.DriveReadonly};
        private const string ApplicationName = "Drive API .NET Quickstart";
        
        protected readonly DriveService DriveService;

        protected GoogleDrive()
        {
            UserCredential credential;

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                var credPath = "token.json";

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Drive API service.
            DriveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        public void Download()
        {
            var fileId = "0BwwA4oUTeiV1UVNwOHItT0xfa2M";
            var request = DriveService.Files.Get(fileId);
            var stream = new MemoryStream();

            // Add a handler which will be notified on progress changes.
            // It will notify on each chunk download and when the
            // download is completed or failed.
            request.MediaDownloader.ProgressChanged +=
                progress =>
                {
                    switch (progress.Status)
                    {
                        case DownloadStatus.Downloading:
                        {
                            Console.WriteLine(progress.BytesDownloaded);
                            break;
                        }
                        case DownloadStatus.Completed:
                        {
                            Console.WriteLine("Download complete.");
                            break;
                        }
                        case DownloadStatus.Failed:
                        {
                            Console.WriteLine("Download failed.");
                            break;
                        }
                    }
                };

            request.Download(stream);
        }
    }
}