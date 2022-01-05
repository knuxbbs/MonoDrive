using System;
using System.IO;

namespace MonoDrive.Application.Models
{
    public class LocalDirectoryInfo
    {
        /// <summary>
        /// Gets or sets the attributes for the current file or directory.
        /// </summary>
        public FileAttributes Attributes { get; set; }

        /// <summary>
        /// Gets a value indicating whether the directory exists.
        /// </summary>
        public bool Exists { get; set; }
        
        /// <summary>
        /// Gets the full path of the directory or file.
        /// </summary>
        public string FullName { get; set; }
        
        /// <summary>
        /// Gets or sets the creation time, in coordinated universal time (UTC), of the current file or directory.
        /// </summary>
        public DateTimeOffset CreationTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the time, in coordinated universal time (UTC), when the current file or directory was last written to.
        /// </summary>
        public DateTimeOffset LastWriteTimeUtc { get; set; }
        
        /// <summary>
        /// Gets or sets the time, in coordinated universal time (UTC), that the current file or directory was last accessed.
        /// </summary>
        public DateTimeOffset LastAccessTimeUtc { get; set; }
        
        public string RemoteId { get; set; }
        
        public string ParentRemoteId { get; set; }
    }
}