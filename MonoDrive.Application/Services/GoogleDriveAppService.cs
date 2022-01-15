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

            //A pasta raíz não é retornada pela consulta de diretórios
            var rootDirectoryId = await GetRootRemoteDirectoryId(cancellationToken);

            //Pastas compartilhadas são retornadas como órfãs
            remoteDirectories = remoteDirectories.Where(x => x.Parents != null).ToArray();

            var collection = _liteDatabaseAsync.GetCollection<LocalDirectoryInfo>();
            var localDirectoriesInfo = await collection.Query().ToListAsync();

            var newDirectories = FilterNewRemoteDirectories(localDirectoriesInfo, remoteDirectories);

            var directoriesInfo =
                CreateDirectories(parentDirectoryPath, rootDirectoryId, localDirectoriesInfo, newDirectories);

            //TODO: Remover diretórios excluídos do servidor remoto
            await InsertLocalDirectoriesInfo(directoriesInfo);

            //var directoriesInfo = parentDirectoryInfo.GetDirectories("*.*", SearchOption.AllDirectories);

            _logger.LogInformation(
                "{Count} new directories successfully created. Elapsed time: {Milliseconds} milliseconds",
                directoriesInfo.Count, stopwatch.Elapsed.Milliseconds);
        }

        private async Task InsertLocalDirectoriesInfo(ICollection<LocalDirectoryInfo> directoriesInfo)
        {
            if (directoriesInfo.Count <= 0)
            {
                return;
            }

            var collection = _liteDatabaseAsync.GetCollection<LocalDirectoryInfo>();

            await collection.EnsureIndexAsync(x => x.RemoteId, true);
            await collection.InsertBulkAsync(directoriesInfo);
        }

        private static List<LocalDirectoryInfo> CreateDirectories(string parentDirectoryPath, string rootDirectoryId,
            ICollection<LocalDirectoryInfo> localDirectoriesInfo,
            ICollection<GoogleDriveFileMetadata> remoteDirectories)
        {
            var builder = new DirectoryTreeBuilder(parentDirectoryPath, rootDirectoryId, remoteDirectories);
            var root = builder.Root;

            root.Traverse(x =>
            {
                x.Create();
            });

            return root.Flatten().ToList();
        }

        private static IList<GoogleDriveFileMetadata> FilterNewRemoteDirectories(
            IEnumerable<LocalDirectoryInfo> localDirectoriesInfo,
            IEnumerable<GoogleDriveFileMetadata> remoteDirectories)
        {
            return remoteDirectories.Where(x => localDirectoriesInfo.All(y => y.RemoteId != x.Id)).ToArray();
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
        private async Task<IList<GoogleDriveFileMetadata>> GetRemoteDirectoriesTree(
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();

            var listRequest = _driveService.Files.List();
            listRequest.PageSize = 1000;
            listRequest.Fields = "nextPageToken, files(id, name, mimeType, parents, trashed, shared)";

            var query = new StringBuilder("mimeType = 'application/vnd.google-apps.folder' ")
                .Append("and trashed = false ");

            listRequest.Q = query.ToString();

            var files = new List<GoogleDriveFileMetadata>();
            FileList fileList = null;

            while (fileList == null || !string.IsNullOrWhiteSpace(fileList.NextPageToken))
            {
                if (fileList != null)
                {
                    listRequest.PageToken = fileList.NextPageToken;
                }

                fileList = await listRequest.ExecuteAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                files.AddRange(fileList.Files.Select(x => new GoogleDriveFileMetadata
                {
                    Id = x.Id,
                    Name = x.Name,
                    Parents = x.Parents
                }));
            }

            _logger.LogInformation(
                "{Count} directories downloaded. Elapsed time: {Milliseconds} milliseconds", files.Count,
                stopwatch.Elapsed.Milliseconds);

            return files;
        }
    }
}