using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using MonoDrive.Application.Interfaces;

namespace MonoDrive.Application.Presenters
{
    public class MainWindowPresenter : IMainWindowPresenter
    {
        private readonly IGoogleOAuthAppService _oAuthAppService;
        private readonly IGoogleDriveAppService _driveAppService;
        
        public MainWindowPresenter(IGoogleOAuthAppService oAuthAppService, 
            IGoogleDriveAppService driveAppService)
        {
            _oAuthAppService = oAuthAppService;
            _driveAppService = driveAppService;
        }
        
        //public int RequestsCount = 0;

        private Stopwatch _stopwatch;

        private string _localRootFolder;

        public async Task<string> GetUserEmail()
        {
            return (await _oAuthAppService.GetUserInfo()).Email;
        }

        public void Sync(string folderPath)
        {
            const string folderName = "Teste";
            _localRootFolder = Path.Combine(folderPath, folderName);

            Directory.CreateDirectory(_localRootFolder);

            _stopwatch = new Stopwatch();
            _stopwatch.Start();

            //TODO: Salvar informações e configurações do usuário
            //TODO: Obter estrutura de diretórios
            _driveAppService.DownloadFolderStructure("root",
                _localRootFolder);
            //await DownloadFiles();
        }

//        private async Task DownloadFolderStructure(string remoteFolderName,
//            string parentFolderPath)
//        {
//            // Define parameters of request.
//            var listRequest = DriveService.Files.List();
//            listRequest.Fields = "nextPageToken, files(id, name, mimeType, parents, trashed)";
//
//            var query = new StringBuilder("mimeType = 'application/vnd.google-apps.folder' ")
//                .Append("and trashed = false ")
//                .AppendFormat($"and '{remoteFolderName}' in parents");
//
//            listRequest.Q = query.ToString();
//
//            FileList filesList;
//
//            try
//            {
//                // List files.
//                filesList = await listRequest.ExecuteAsync();
//            }
//            catch
//            {
//                var errorMessage = new StringBuilder($"Requests per second: {RequestsPerSecond}. ")
//                    .AppendFormat($"Requests count: {RequestsCount}. ")
//                    .AppendFormat($"Elapsed time: {_stopwatch.Elapsed.Seconds} seconds.");
//
//                throw new Exception(errorMessage.ToString());
//            }
//
//            var files = filesList.Files;
//            //PageStreamer<>
//
//            RequestsCount++;
//
//            await Task.WhenAll(files.Select(async x =>
//            {
//                var localFolderPath = Path.Combine(parentFolderPath, x.Name);
//
//                Directory.CreateDirectory(localFolderPath);
//
//                if (IsRequestCountAtMax)
//                {
//                    await Task.Delay(1000);
//                }
//
//                await DownloadFolderStructure(x.Id,
//                    localFolderPath);
//            }));
//        }
//
//        private async Task DownloadFiles()
//        {
//            // Define parameters of request.
//            var listRequest = DriveService.Files.List();
//            listRequest.PageSize = 10;
//            listRequest.Fields = "nextPageToken, files(id, name)";
//
//            // List files.
//            var filesList = await listRequest.ExecuteAsync();
//            var files = filesList.Files;
//
//            // Download files
//            await Task.WhenAll(files.Select(async x =>
//            {
//                var request = DriveService.Files.Get(x.Id);
//
//                // Add a handler which will be notified on progress changes.
//                // It will notify on each chunk download and when the
//                // download is completed or failed.
//                request.MediaDownloader.ProgressChanged += Changed;
//
//                using (var memoryStream = new MemoryStream())
//                {
//                    await request.DownloadAsync(memoryStream);
//
//                    var filePath = Path.Combine(_localRootFolder, x.Name);
//
//                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
//                    {
//                        memoryStream.Position = 0;
//                        await memoryStream.CopyToAsync(fileStream);
//                    }
//                }
//            }));
//        }
//
//        public float RequestsPerSecond
//        {
//            get
//            {
//                if (_stopwatch != null && _stopwatch.IsRunning)
//                {
//                    return (float)RequestsCount / (_stopwatch.ElapsedMilliseconds * 1000);
//                }
//
//                return 0;
//            }
//        }
//
//        public bool IsRequestCountAtMax => RequestsPerSecond >= 10;
//
//        private static void Changed(IDownloadProgress progress)
//        {
//            switch (progress.Status)
//            {
//                case DownloadStatus.Downloading:
//                    {
//                        Console.WriteLine(progress.BytesDownloaded);
//                        break;
//                    }
//                case DownloadStatus.Completed:
//                    {
//                        Console.WriteLine("Download complete.");
//                        break;
//                    }
//                case DownloadStatus.Failed:
//                    {
//                        Console.WriteLine("Download failed.");
//                        break;
//                    }
//                case DownloadStatus.NotStarted:
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException();
//            }
//        }
    }
}