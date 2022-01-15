using System.IO;
using LiteDB;

namespace MonoDrive.Application.Models
{
    public class LocalDirectoryInfo
    {
        /// <summary>
        /// DirectoryInfo implementation: https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/IO/DirectoryInfo.cs
        /// </summary>
        private DirectoryInfo _directoryInfo;
        
        public LocalDirectoryInfo(string path, string remoteId)
        {
            _directoryInfo = new DirectoryInfo(path);
            Snapshot = new FileSystemInfoSnapshot(_directoryInfo);
            RemoteId = remoteId;
        }

        public LocalDirectoryInfo(string path, GoogleDriveFileMetadata fileMetadata)
        {
            _directoryInfo = new DirectoryInfo(path);
            Snapshot = new FileSystemInfoSnapshot(_directoryInfo);
            RemoteId = fileMetadata.Id;
            ParentRemoteId = fileMetadata.Parents[0];
        }
        
        // ReSharper disable once UnusedMember.Global
        [BsonCtor]
        public LocalDirectoryInfo(string fullName)
        {
            _directoryInfo = new DirectoryInfo(fullName);
        }

        public string FullName => _directoryInfo.FullName;

        public FileSystemInfoSnapshot Snapshot { get; set; }

        public string RemoteId { get; set; }
        
        public string ParentRemoteId { get; set; }

        public void Create()
        {
            if (_directoryInfo.Exists)
            {
                return;
            }
            
            _directoryInfo.Create();
            _directoryInfo = new DirectoryInfo(_directoryInfo.FullName);
            Snapshot = new FileSystemInfoSnapshot(_directoryInfo);
        }

        public override string ToString()
        {
            return _directoryInfo.FullName;
        }
    }
}