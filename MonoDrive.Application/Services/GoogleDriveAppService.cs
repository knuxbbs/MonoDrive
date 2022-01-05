using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using LiteDB;
using LiteDB.Async;
using Microsoft.Extensions.Logging;
using MonoDrive.Application.Interfaces;
using MonoDrive.Application.Models;
using GoogleDriveFile = Google.Apis.Drive.v3.Data.File;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace MonoDrive.Application.Services
{
    //Talvez isso aqui deva ser uma facade
    public class GoogleDriveAppService : IGoogleDriveAppService
    {
        private readonly DriveService _driveService;
        private readonly ILiteDatabaseAsync _liteDatabaseAsync;
        private readonly ILogger _logger;

        public GoogleDriveAppService(IGoogleApiServiceProvider serviceProvider, ILiteDatabaseAsync liteDatabaseAsync,
            ILogger<GoogleDriveAppService> logger)
        {
            _driveService = serviceProvider.GetDriveService();
            // _driveService.HttpClient.MessageHandler.LogEvents = ConfigurableMessageHandler.LogEventType.RequestHeaders |
            //                                                     ConfigurableMessageHandler.LogEventType.ResponseBody;
            _liteDatabaseAsync = liteDatabaseAsync;
            _logger = logger;
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

        /// <summary>
        /// Baixa estrutura de diretórios e a adiciona ao caminho passado como parâmetro
        /// </summary>
        /// <param name="parentDirectoryPath">Caminho no qual será reproduzida a estrutura de diretórios remotos</param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="OperationCanceledException"></exception>
        /// <returns></returns>
        public async Task DownloadAndCreateDirectories(string parentDirectoryPath,
            CancellationToken cancellationToken = default)
        {
            //TODO: Este método pode ser desacoplado do Google Drive
            //TODO: Tratar possíveis excessões relacionadas à construção do DirectoryInfo antes de baixar as informações
            //var parentDirectoryInfo = new DirectoryInfo(parentDirectoryPath);

            var remoteDirectories = await GetRemoteDirectoriesTree(cancellationToken);

            var stopwatch = Stopwatch.StartNew();

            //Pastas compartilhadas são retornadas como órfãs
            remoteDirectories = remoteDirectories.Where(x => x.Parents != null).ToArray();

            //A pasta raíz não é retornada pela consulta de diretórios
            var rootDirectoryId = await GetRootRemoteDirectoryId(cancellationToken);

            var newDirectories = await FilterNewRemoteDirectories(remoteDirectories, rootDirectoryId);

            var directoriesInfo = CreateDirectories(parentDirectoryPath, newDirectories.ToArray());

            //TODO: Remover diretórios excluídos do servidor remoto
            await SaveLocalDirectoriesInfo(directoriesInfo);

            //var directoriesInfo = parentDirectoryInfo.GetDirectories("*.*", SearchOption.AllDirectories);

            _logger.LogInformation(
                "{Count} new directories successfully created. Elapsed time: {Milliseconds} milliseconds",
                directoriesInfo.Count, stopwatch.Elapsed.Milliseconds);
        }

        private async Task SaveLocalDirectoriesInfo(ICollection<LocalDirectoryInfo> directoriesInfo)
        {
            if (directoriesInfo.Count <= 0)
            {
                return;
            }

            var collection = _liteDatabaseAsync.GetCollection<LocalDirectoryInfo>();

            await collection.EnsureIndexAsync(x => x.RemoteId, true);
            await collection.InsertBulkAsync(directoriesInfo);
        }

        private List<LocalDirectoryInfo> CreateDirectories(string parentDirectoryPath,
            ICollection<GoogleDriveFile> remoteDirectories)
        {
            var directoriesInfo = new List<LocalDirectoryInfo>();

            void CreateDirectory(GoogleDriveFile remoteDirectory, string localPath)
            {
                var localDirectoryPath = Path.Combine(localPath, remoteDirectory.Name);

                var directoryInfo = new DirectoryInfo(localDirectoryPath);
                directoryInfo.Create();
                directoriesInfo.Add(new LocalDirectoryInfo
                {
                    Attributes = directoryInfo.Attributes,
                    Exists = directoryInfo.Exists,
                    FullName = directoryInfo.FullName,
                    CreationTimeUtc = directoryInfo.CreationTimeUtc,
                    LastWriteTimeUtc = directoryInfo.LastWriteTimeUtc,
                    LastAccessTimeUtc = directoryInfo.LastAccessTimeUtc,
                    RemoteId = remoteDirectory.Id,
                    ParentRemoteId = remoteDirectory.Parents[0]
                });

                var childrenDirectories = remoteDirectories.Where(y => y.Parents.Contains(remoteDirectory.Id));

                foreach (var directory in childrenDirectories)
                {
                    CreateDirectory(directory, localDirectoryPath);
                }
            }

            foreach (var directory in remoteDirectories)
            {
                CreateDirectory(directory, parentDirectoryPath);
            }

            return directoriesInfo;
        }

        // private List<LocalDirectoryInfo> CreateDirectories(string parentDirectoryPath,
        //     IEnumerable<GoogleDriveFile> remoteDirectories)
        // {
        //     //var remoteDirectoriesStack = new Stack<GoogleDriveFile>(remoteDirectories);
        //     var directories = new Stack<string>(20);
        //     directories.Push(parentDirectoryPath);
        //
        //     while (directories.Count > 0)
        //     {
        //         var localPath = directories.Pop();
        //         //var localFolderPath = Path.Combine(localPath, remoteFolder.Name);
        //         
        //         var directoryInfo = new DirectoryInfo(localPath);
        //         directoryInfo.Create();
        //         
        //         var children = remoteDirectories.Where(y => y.Parents.Contains(remoteFolder.Id));
        //     }
        //     
        //     return null;
        // }

        private async Task<IEnumerable<GoogleDriveFile>> FilterNewRemoteDirectories(
            IEnumerable<GoogleDriveFile> remoteDirectories, string rootDirectoryId)
        {
            var rootChildren = remoteDirectories.Where(x => x.Parents.Contains(rootDirectoryId));

            var collection = _liteDatabaseAsync.GetCollection<LocalDirectoryInfo>();
            var localDirectoriesInfo = await collection.FindAllAsync();

            return rootChildren.Where(x => localDirectoriesInfo.All(y => y.RemoteId != x.Id));
        }

        private async Task<string> GetRootRemoteDirectoryId(CancellationToken cancellationToken = default)
        {
            var collection = _liteDatabaseAsync.GetCollection("RootDirectoryInfo");
            var bsonDocument = await collection.FindOneAsync("$._id = 1");

            if (bsonDocument != null)
            {
                return bsonDocument["RemoteId"].AsString;
            }

            var rootDirectory = await _driveService.Files.Get("root").ExecuteAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            await collection.InsertAsync(new BsonDocument {["_id"] = 1, ["RemoteId"] = rootDirectory.Id});

            return rootDirectory.Id;
        }

        /// <summary>
        /// Baixa estrutura de diretórios 
        /// </summary>
        /// <returns></returns>
        private async Task<IList<GoogleDriveFile>> GetRemoteDirectoriesTree(
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();

            var listRequest = _driveService.Files.List();
            listRequest.PageSize = 1000;
            listRequest.Fields = "nextPageToken, files(id, name, mimeType, parents, trashed, shared)";

            var query = new StringBuilder("mimeType = 'application/vnd.google-apps.folder' ")
                .Append("and trashed = false ");

            listRequest.Q = query.ToString();

            var files = new List<GoogleDriveFile>();
            FileList fileList = null;

            while (fileList == null || !string.IsNullOrWhiteSpace(fileList.NextPageToken))
            {
                if (fileList != null)
                {
                    listRequest.PageToken = fileList.NextPageToken;
                }

                fileList = await listRequest.ExecuteAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                files.AddRange(fileList.Files);
            }

            _logger.LogInformation(
                "{Count} directories downloaded. Elapsed time: {Milliseconds} milliseconds", files.Count,
                stopwatch.Elapsed.Milliseconds);

            return files;
        }
    }
}