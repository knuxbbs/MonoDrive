using System.Text;
using System.Threading.Tasks;
using Google.Apis.Drive.v3;
using ODrive.Sharp.Application.Interfaces;

namespace ODrive.Sharp.Application.Providers
{
    public class GoogleDriveAppService : IGoogleDriveAppService
    {
        private readonly DriveService _driveService;

        public GoogleDriveAppService(IGoogleApisServiceProvider serviceProvider)
        {
            _driveService = serviceProvider.DriveService;
        }

        public async Task DownloadFolderStructure(string remoteFolderName,
            string parentFolderPath)
        {
            // Define parameters of request.
            var listRequest = _driveService.Files.List();
            listRequest.Fields = "nextPageToken, files(id, name, mimeType, parents, trashed)";

            var query = new StringBuilder("mimeType = 'application/vnd.google-apps.folder' ")
                .Append("and trashed = false ")
                .AppendFormat($"and '{remoteFolderName}' in parents");

            listRequest.Q = query.ToString();
        }

        private async Task DownloadFiles()
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