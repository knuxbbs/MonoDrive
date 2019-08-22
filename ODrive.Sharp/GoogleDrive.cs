using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private static readonly string[] Scopes =
        {
            DriveService.Scope.DriveReadonly
        };
        
        private const string ApplicationName = "knuxbbs Open Drive";

        private static string _folderPath;

        protected readonly DriveService DriveService;

        public GoogleDrive()
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

        public async Task Sync(string folderPath)
        {
            const string folderName = "Teste";
            _folderPath = Path.Combine(folderPath, folderName);

            Directory.CreateDirectory(_folderPath);

            await Download();
        }

        private async Task Download()
        {
            // Define parameters of request.
            var listRequest = DriveService.Files.List();
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name)";

            // List files.
            var files = listRequest.Execute().Files;

            var count = 0;
            
            // Download files
            await Task.WhenAll(files.Select(async x =>
            {
                var request = DriveService.Files.Get(x.Id);
                
                // Add a handler which will be notified on progress changes.
                // It will notify on each chunk download and when the
                // download is completed or failed.
                request.MediaDownloader.ProgressChanged += Changed;

                using (var memoryStream = new MemoryStream())
                {
                    await request.DownloadAsync(memoryStream);

                    var filePath = Path.Combine(_folderPath, $"file{count++}");
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        await memoryStream.CopyToAsync(fileStream);
                    }
                }
            }));
        }

        private static void Changed(IDownloadProgress progress)
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
                case DownloadStatus.NotStarted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}