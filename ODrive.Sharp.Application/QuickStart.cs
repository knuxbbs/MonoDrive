using System;
using System.Linq;
using Google.Apis.Drive.v3;
using ODrive.Sharp.Application.Services;

namespace ODrive.Sharp.Application
{
    public class QuickStart
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