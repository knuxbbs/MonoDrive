using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            ILogger<GoogleDriveAppService> logger)
        {
            _driveService = serviceProvider.GetDriveService();
            // _driveService.HttpClient.MessageHandler.LogEvents = ConfigurableMessageHandler.LogEventType.RequestHeaders |
            //                                                     ConfigurableMessageHandler.LogEventType.ResponseBody;

            _logger = logger;
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

        public async Task CreateFolders(string parentDirectoryPath)
        {
            //TODO: Tratar possíveis excessões relacionadas à construção do DirectoryInfo antes de baixar as informações
            //var parentDirectoryInfo = new DirectoryInfo(parentDirectoryPath);

            var folders = await DownloadFoldersStructure();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //Pastas compartilhadas são retornadas como órfãs
            folders = folders.Where(x => x.Parents != null).ToArray();
            //TODO: Verificar se é interessante salvar o id da pasta raíz
            //A pasta raíz não é retornada pela consulta de diretórios
            var rootFolder = await _driveService.Files.Get("root").ExecuteAsync();

            var rootChildren = folders.Where(x => x.Parents.Contains(rootFolder.Id));

            var directoriesIds = new Dictionary<string, DirectoryInfo>();

            void CreateFolder(File remoteFolder, string localPath)
            {
                var localFolderPath = Path.Combine(localPath, remoteFolder.Name);
                
                var directoryInfo = new DirectoryInfo(localFolderPath);
                directoryInfo.Create();
                directoriesIds.Add(remoteFolder.Id, directoryInfo);

                var children = folders.Where(y => y.Parents.Contains(remoteFolder.Id));

                foreach (var childrenFolder in children)
                {
                    CreateFolder(childrenFolder, localFolderPath);
                }
            }

            foreach (var folder in rootChildren)
            {
                CreateFolder(folder, parentDirectoryPath);
            }

            //var directoriesInfo = parentDirectoryInfo.GetDirectories("*.*", SearchOption.AllDirectories);

            _logger.LogInformation(
                $"Directories successfully created. Elapsed time: {stopwatch.Elapsed.Milliseconds} milliseconds.");
        }

        private async Task<IList<File>> DownloadFoldersStructure()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var listRequest = _driveService.Files.List();
            listRequest.PageSize = 1000;
            listRequest.Fields = "nextPageToken, files(id, name, mimeType, parents, trashed, shared)";

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

            _logger.LogInformation(
                $"{files.Count} folders downloaded. Elapsed time: {stopwatch.Elapsed.Seconds} seconds.");

            return files;
        }
    }
}