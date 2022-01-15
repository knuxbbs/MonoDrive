using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MonoDrive.Application.Models;

public class DirectoryTreeBuilder
{
    private string _parentDirectoryPath;
    private string _rootDirectoryId;
    private ICollection<GoogleDriveFileMetadata> _remoteDirectories;

    public DirectoryTreeBuilder(string parentDirectoryPath, string rootDirectoryId,
        ICollection<GoogleDriveFileMetadata> remoteDirectories)
    {
        _parentDirectoryPath = parentDirectoryPath;
        _rootDirectoryId = rootDirectoryId;
        _remoteDirectories = remoteDirectories;

        Root = BuildTree();
    }

    public TreeNode<LocalDirectoryInfo> Root { get; }

    private TreeNode<LocalDirectoryInfo> BuildTree()
    {
        var rootTreeNode = new TreeNode<LocalDirectoryInfo>(new LocalDirectoryInfo(_parentDirectoryPath, _rootDirectoryId));
        var rootRemoteChildren = GetRootRemoteChildren();

        void AddDirectory(GoogleDriveFileMetadata remoteDirectory, TreeNode<LocalDirectoryInfo> node)
        {
            var localDirectoryPath = Path.Combine(node.Value.FullName, remoteDirectory.Name);

            var localDirectoryInfo = new LocalDirectoryInfo(localDirectoryPath, remoteDirectory);

            var childNode = node.AddChild(localDirectoryInfo);

            var childrenDirectories = _remoteDirectories.Where(y => y.Parents.Contains(remoteDirectory.Id));

            foreach (var directory in childrenDirectories)
            {
                AddDirectory(directory, childNode);
            }
        }

        foreach (var directory in rootRemoteChildren)
        {
            AddDirectory(directory, rootTreeNode);
        }

        return rootTreeNode;
    }

    private IEnumerable<GoogleDriveFileMetadata> GetRootRemoteChildren()
    {
        return _remoteDirectories.Where(x => x.Parents.Contains(_rootDirectoryId));
    }
}