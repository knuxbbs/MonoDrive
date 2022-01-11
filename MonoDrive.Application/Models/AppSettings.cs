namespace MonoDrive.Application.Models;

public class AppSettings
{
    /// <summary>
    /// Propriedade necessária para o LiteDB
    /// </summary>
    public int Id { get; set; } = 1;
    public string LocalRootDirectory { get; set; }
}