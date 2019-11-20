using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using ODrive.Sharp.Application.Interfaces;
using FileMode = System.IO.FileMode;

namespace ODrive.Sharp.Application.Services
{
    public class GoogleDriveAppService : IGoogleDriveAppService
    {
        private const string CredPath = "token.json";

        // The file token.json stores the user's access and refresh tokens, and is created
        // automatically when the authorization flow completes for the first time.
        private static readonly string[] Scopes =
        {
            Oauth2Service.Scope.UserinfoEmail,
            DriveService.Scope.DriveReadonly
        };

        private UserCredential _userCredential;

        private Userinfoplus _userInfo;

        private static string _localRootFolder;

        private BaseClientService.Initializer BaseClientService => new BaseClientService.Initializer
        {
            HttpClientInitializer = _userCredential,
            ApplicationName = "knuxbbs Open Drive"
        };

        protected DriveService DriveService => GetDriveService().Result;

        private DriveService _driveService;

        private Stopwatch _stopwatch;

        public int RequestsCount = 0;

        private async Task<DriveService> GetDriveService()
        {
            if (_driveService != null)
            {
                return _driveService;
            }

            if (_userCredential == null)
            {
                await SignIn();
            }

            // Create Drive API service.
            _driveService = new DriveService(BaseClientService);

            return _driveService;
        }

        public string Email => _userInfo.Email;

        public async Task SignIn()
        {
            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                _userCredential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(CredPath, true));
            }

            var oauthService = new Oauth2Service(BaseClientService);
            _userInfo = await oauthService.Userinfo.Get().ExecuteAsync();

            //TODO: Salvar credenciais do usuário
//            using (var db = new LiteDatabase(@"MyData.db"))
//            {
//                var collection = db.GetCollection("account");
//            }
        }

        public async Task Sync(string folderPath)
        {
            const string folderName = "Teste";
            _localRootFolder = Path.Combine(folderPath, folderName);

            Directory.CreateDirectory(_localRootFolder);

            _stopwatch = new Stopwatch();
            _stopwatch.Start();

            //TODO: Salvar informações e configurações do usuário
            //TODO: Obter estrutura de diretórios
            await DownloadFolderStructure("root",
                _localRootFolder);
            //await DownloadFiles();
        }

        private async Task DownloadFolderStructure(string remoteFolderName,
            string parentFolderPath)
        {
            // Define parameters of request.
            var listRequest = DriveService.Files.List();
            listRequest.Fields = "nextPageToken, files(id, name, mimeType, parents, trashed)";

            var query = new StringBuilder("mimeType = 'application/vnd.google-apps.folder' ")
                .Append("and trashed = false ")
                .AppendFormat($"and '{remoteFolderName}' in parents");

            listRequest.Q = query.ToString();

            FileList filesList;

            try
            {
                // List files.
                filesList = await listRequest.ExecuteAsync();
            }
            catch
            {
                var errorMessage = new StringBuilder($"Requests per second: {RequestsPerSecond}. ")
                    .AppendFormat($"Requests count: {RequestsCount}. ")
                    .AppendFormat($"Elapsed time: {_stopwatch.Elapsed.Seconds} seconds.");

                throw new Exception(errorMessage.ToString());
            }

            var files = filesList.Files;
            //PageStreamer<>

            RequestsCount++;

            await Task.WhenAll(files.Select(async x =>
            {
                var localFolderPath = Path.Combine(parentFolderPath, x.Name);

                Directory.CreateDirectory(localFolderPath);

                if (IsRequestCountAtMax)
                {
                    await Task.Delay(1000);
                }

                await DownloadFolderStructure(x.Id,
                    localFolderPath);
            }));
        }

        private async Task DownloadFiles()
        {
            // Define parameters of request.
            var listRequest = DriveService.Files.List();
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name)";

            // List files.
            var filesList = await listRequest.ExecuteAsync();
            var files = filesList.Files;

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

                    var filePath = Path.Combine(_localRootFolder, x.Name);

                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        memoryStream.Position = 0;
                        await memoryStream.CopyToAsync(fileStream);
                    }
                }
            }));
        }

        public float RequestsPerSecond
        {
            get
            {
                if (_stopwatch != null && _stopwatch.IsRunning)
                {
                    return (float) RequestsCount / (_stopwatch.ElapsedMilliseconds * 1000);
                }

                return 0;
            }
        }

        public bool IsRequestCountAtMax => RequestsPerSecond >= 10;

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