namespace MonoDrive.Application.Models;

public class AppSettings
{
    /// <summary>
    /// Propriedade necessária para o LiteDB
    /// </summary>
    public int Id { get; set; }
    public string LocalRootDirectory { get; set; }
}