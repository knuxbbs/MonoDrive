using System;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Drive.v3;
using ODrive.Sharp.Application.Interfaces;

namespace ODrive.Sharp.Application.Services
{
    public class GoogleDriveAppService : IGoogleDriveAppService
    {
        private readonly Lazy<Task<DriveService>> _driveService;

        public GoogleDriveAppService(IGoogleApiServiceProvider serviceProvider)
        {
            _driveService = new Lazy<Task<DriveService>>(async () => 
                await serviceProvider.GetDriveService());
        }

        public async Task DownloadFolderStructure(string remoteFolderName,
            string parentFolderPath)
        {
            // Define parameters of request.
            var listRequest = (await _driveService.Value).Files.List();
            listRequest.Fields = "nextPageToken, files(id, name, mimeType, parents, trashed)";

            var query = new StringBuilder("mimeType = 'application/vnd.google-apps.folder' ")
                .Append("and trashed = false ")
                .AppendFormat($"and '{remoteFolderName}' in parents");

            listRequest.Q = query.ToString();
        }

        public async Task DownloadFiles()
        {
            // Define parameters of request.
            var listRequest = (await _driveService.Value).Files.List();
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name)";

            // List files.
            var filesList = await listRequest.ExecuteAsync();
            var files = filesList.Files;
        }
    }
}