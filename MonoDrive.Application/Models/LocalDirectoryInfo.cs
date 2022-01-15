using System;
using System.IO;

namespace MonoDrive.Application.Models
{
    public class LocalDirectoryInfo
    {
        /// <summary>
        /// DirectoryInfo implementation: https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/IO/DirectoryInfo.cs
        /// </summary>
        private DirectoryInfo _directoryInfo;
        
        public LocalDirectoryInfo()
        {
        }

        public LocalDirectoryInfo(string path, string remoteId)
        {
            _directoryInfo = new DirectoryInfo(path);
            RemoteId = remoteId;
        }

        public LocalDirectoryInfo(string path, GoogleDriveFileMetadata fileMetadata)
        {
            _directoryInfo = new DirectoryInfo(path);
            RemoteId = fileMetadata.Id;
            ParentRemoteId = fileMetadata.Parents[0];
        }

        /// <summary>
        /// Gets or sets the attributes for the current file or directory.
        /// </summary>
        public FileAttributes Attributes => _directoryInfo.Attributes;

        /// <summary>
        /// Gets a value indicating whether the directory exists.
        /// </summary>
        public bool Exists => _directoryInfo.Exists;

        /// <summary>
        /// Gets the full path of the directory or file.
        /// </summary>
        public string FullName => _directoryInfo.FullName;

        /// <summary>
        /// Gets or sets the creation time, in coordinated universal time (UTC), of the current file or directory.
        /// </summary>
        public DateTimeOffset CreationTimeUtc => _directoryInfo.CreationTimeUtc;

        /// <summary>
        /// Gets or sets the time, in coordinated universal time (UTC), when the current file or directory was last written to.
        /// </summary>
        public DateTimeOffset LastWriteTimeUtc => _directoryInfo.LastWriteTimeUtc;

        /// <summary>
        /// Gets or sets the time, in coordinated universal time (UTC), that the current file or directory was last accessed.
        /// </summary>
        public DateTimeOffset LastAccessTimeUtc => _directoryInfo.LastAccessTimeUtc;

        public string RemoteId { get; set; }
        
        public string ParentRemoteId { get; set; }

        public void Create()
        {
            if (_directoryInfo.Exists)
            {
                return;
            }
            
            _directoryInfo.Create();
            _directoryInfo = new DirectoryInfo(FullName);
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}