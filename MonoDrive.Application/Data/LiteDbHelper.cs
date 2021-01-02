using System;
using System.IO;

namespace MonoDrive.Application.Data
{
    public class LiteDbHelper
    {
        public static string GetFilePath(string file)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(path, file);
        }
    }
}