using System;
using System.IO;
using System.Linq;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace ODrive.Sharp
{
    public class GoogleDriveQuickStart : GoogleDrive
    {
        public void Run()
        {
            // Define parameters of request.
            var listRequest = DriveService.Files.List();
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name)";

            // List files.
            var files = listRequest.Execute().Files;

            Console.WriteLine("Files:");

            if (files.Any())
            {
                foreach (var file in files)
                {
                    Console.WriteLine($"{file.Name} ({file.Id})");
                }
            }
            else
            {
                Console.WriteLine("No files found.");
            }

            //Console.Read();
        }
    }
}