namespace MonoDrive.Application.Models;

public class DirectoryTreeNode : TreeNode<LocalDirectoryInfo>
{
    public DirectoryTreeNode(LocalDirectoryInfo value) : base(value)
    {
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}