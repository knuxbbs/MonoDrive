using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Microsoft.Extensions.Logging;
using MonoDrive.Application.Interfaces;
using File = Google.Apis.Drive.v3.Data.File;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace MonoDrive.Application.Services
{
    public class GoogleDriveAppService : IGoogleDriveAppService
    {
        private readonly DriveService _driveService;
        private readonly ILogger _logger;

        public GoogleDriveAppService(IGoogleApiServiceProvider serviceProvider,
            ILogger<GoogleOAuthAppService> logger)
        {
            _driveService = serviceProvider.GetDriveService();
            // _driveService.HttpClient.MessageHandler.LogEvents = ConfigurableMessageHandler.LogEventType.RequestHeaders |
            //                                                     ConfigurableMessageHandler.LogEventType.ResponseBody;

            _logger = logger;
        }

        public async Task GetFoldersStructure()
        {
            var listRequest = _driveService.Files.List();
            listRequest.PageSize = 1000;
            listRequest.Fields = "nextPageToken, files(id, name, mimeType, parents, trashed)";

            var query = new StringBuilder("mimeType = 'application/vnd.google-apps.folder' ")
                .Append("and trashed = false ");

            listRequest.Q = query.ToString();

            var files = new List<File>();
            FileList fileList = null;

            while (fileList == null || !string.IsNullOrWhiteSpace(fileList.NextPageToken))
            {
                if (fileList != null)
                {
                    listRequest.PageToken = fileList.NextPageToken;
                }

                fileList = await listRequest.ExecuteAsync();
                files.AddRange(fileList.Files);
            }

            var filesCount = files.Count;
        }

        public async Task DownloadFolderStructure(string remoteFolderName,
            string parentFolderPath)
        {
            var listRequest = _driveService.Files.List();
            listRequest.PageSize = 1000;
            //TODO: Pass nextPageToken. https://stackoverflow.com/q/41572228/7267592
            listRequest.Fields = "nextPageToken, files(id, name, mimeType, parents, trashed)";

            var query = new StringBuilder("mimeType = 'application/vnd.google-apps.folder' ")
                .Append("and trashed = false ")
                .AppendFormat($"and '{remoteFolderName}' in parents");

            listRequest.Q = query.ToString();

            var fileList = await listRequest.ExecuteAsync();
            var files = fileList.Files;

            await Task.WhenAll(files.Select(async x =>
            {
                var localFolderPath = Path.Combine(parentFolderPath, x.Name);

                Directory.CreateDirectory(localFolderPath);
                _logger.LogInformation($"Diretório '{localFolderPath}' criado com sucesso.");

                await DownloadFolderStructure(x.Id,
                    localFolderPath);
            }));
        }

        public async Task DownloadFiles()
        {
            // Define parameters of request.
            var listRequest = _driveService.Files.List();
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name)";

            // List files.
            var filesList = await listRequest.ExecuteAsync();
            var files = filesList.Files;
        }
    }
}