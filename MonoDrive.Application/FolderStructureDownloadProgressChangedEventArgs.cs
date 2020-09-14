using System;

namespace MonoDrive.Application
{
    public abstract class FolderStructureDownloadProgressChangedEventArgs : EventArgs
    {
        public int TotalFolders { get; set; }

        public int CompletedFolders { get; set; }
    }
}