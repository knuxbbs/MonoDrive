using System;
using System.IO;
using LiteDB;

namespace MonoDrive.Application.Models;

public class FileSystemInfoSnapshot
{
    public FileSystemInfoSnapshot(FileSystemInfo directoryInfo)
    {
        Attributes = directoryInfo.Attributes;
        Exists = directoryInfo.Exists;
        FullName = directoryInfo.FullName;
        CreationTimeUtc = directoryInfo.CreationTimeUtc;
        LastWriteTimeUtc = directoryInfo.LastWriteTimeUtc;
        LastAccessTimeUtc = directoryInfo.LastAccessTimeUtc;
    }

    // ReSharper disable once UnusedMember.Global
    [BsonCtor]
    public FileSystemInfoSnapshot(FileAttributes attributes, bool exists, string fullName,
        DateTimeOffset creationTimeUtc, DateTimeOffset lastWriteTimeUtc, DateTimeOffset lastAccessTimeUtc)
    {
        Attributes = attributes;
        Exists = exists;
        FullName = fullName;
        CreationTimeUtc = creationTimeUtc;
        LastWriteTimeUtc = lastWriteTimeUtc;
        LastAccessTimeUtc = lastAccessTimeUtc;
    }

    /// <summary>
    /// Gets or sets the attributes for the current file or directory.
    /// </summary>
    public FileAttributes Attributes { get; }

    /// <summary>
    /// Gets a value indicating whether the directory exists.
    /// </summary>
    public bool Exists { get; }

    /// <summary>
    /// Gets the full path of the directory or file.
    /// </summary>
    public string FullName { get; }

    /// <summary>
    /// Gets or sets the creation time, in coordinated universal time (UTC), of the current file or directory.
    /// </summary>
    public DateTimeOffset CreationTimeUtc { get; }

    /// <summary>
    /// Gets or sets the time, in coordinated universal time (UTC), when the current file or directory was last written to.
    /// </summary>
    public DateTimeOffset LastWriteTimeUtc { get; }

    /// <summary>
    /// Gets or sets the time, in coordinated universal time (UTC), that the current file or directory was last accessed.
    /// </summary>
    public DateTimeOffset LastAccessTimeUtc { get; }
}