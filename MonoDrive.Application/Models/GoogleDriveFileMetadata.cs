using System.Collections.Generic;

namespace MonoDrive.Application.Models;

public class GoogleDriveFileMetadata
{
    /// <summary>The ID of the file.</summary>
    public string Id { get; set; }

    /// <summary>
    /// The name of the file. This is not necessarily unique within a folder. Note that for immutable items such as
    /// the top level folders of shared drives, My Drive root folder, and Application Data folder the name is
    /// constant.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The IDs of the parent folders which contain the file. If not specified as part of a create request, the file
    /// will be placed directly in the user's My Drive folder. If not specified as part of a copy request, the file
    /// will inherit any discoverable parents of the source file. Update requests must use the addParents and
    /// removeParents parameters to modify the parents list.
    /// </summary>
    public IList<string> Parents { get; set; }
}