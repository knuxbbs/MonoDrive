using System;
using System.Threading.Tasks;
using Google;
using Google.Apis.Script.v1;
using Google.Apis.Script.v1.Data;
using MonoDrive.Application.Interfaces;
using Newtonsoft.Json.Linq;

namespace MonoDrive.Application.Services
{
    public class GoogleScriptAppService : IGoogleScriptAppService
    {
        private readonly ScriptService _scriptService;

        public GoogleScriptAppService(IGoogleApiServiceProvider serviceProvider)
        {
            _scriptService = serviceProvider.GetScriptService();
        }

        private const string ScriptId = "1YEpnXKoI7i_LFJcW5szRN_0YO56fPCJII0z6APgKyX5BbyCowZbDoEB-";

        public async Task GetFoldersUnderRoot()
        {
            // Create an execution request object.
            var executionRequest = new ExecutionRequest
            {
                Function = "getFoldersUnderRoot"
            };

            var runRequest = _scriptService.Scripts.Run(executionRequest, ScriptId);

            try
            {
                // Make the API request.
                var operation = await runRequest.ExecuteAsync();

                if (operation.Error == null)
                {
                    // The result provided by the API needs to be cast into
                    // the correct type, based upon what types the Apps
                    // Script function returns. Here, the function returns
                    // an Apps Script Object with String keys and values.
                    // It is most convenient to cast the return value as a JSON
                    // JObject (folderSet).
                    var folderSet = (JObject) operation.Response["result"];

                    if (folderSet.Count == 0)
                    {
                        Console.WriteLine("No folders returned!");
                    }
                    else
                    {
                        Console.WriteLine("Folders under your root folder:");

                        foreach (var folder in folderSet)
                        {
                            Console.WriteLine($"\t{folder.Value} ({folder.Key})");
                        }
                    }
                }
                else
                {
                    // The API executed, but the script returned an error.

                    // Extract the first (and only) set of error details
                    // as a IDictionary. The values of this dictionary are
                    // the script's 'errorMessage' and 'errorType', and an
                    // array of stack trace elements. Casting the array as
                    // a JSON JArray allows the trace elements to be accessed
                    // directly.
                    var error = operation.Error.Details[0];

                    Console.WriteLine("Script error message: {0}", error["errorMessage"]);

                    if (error["scriptStackTraceElements"] != null)
                    {
                        // There may not be a stacktrace if the script didn't
                        // start executing.
                        Console.WriteLine("Script error stacktrace:");

                        var st = (JArray) error["scriptStackTraceElements"];

                        foreach (var trace in st)
                        {
                            Console.WriteLine($"\t{trace["function"]}: {trace["lineNumber"]}");
                        }
                    }
                }
            }
            catch (GoogleApiException e)
            {
                // The API encountered a problem before the script
                // started executing.
                Console.WriteLine("Error calling API:\n{0}", e);
            }
        }
    }
}