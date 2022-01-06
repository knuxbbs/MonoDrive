using System.Threading.Tasks;
using LiteDB.Async;
using MonoDrive.Application.Interfaces;
using MonoDrive.Application.Models;

namespace MonoDrive.Application.Services;

public class SettingsAppService : ISettingsAppService
{
    private readonly ILiteDatabaseAsync _liteDatabaseAsync;

    public SettingsAppService(ILiteDatabaseAsync liteDatabaseAsync)
    {
        _liteDatabaseAsync = liteDatabaseAsync;
    }

    public Task<AppSettings> GetAppSettings()
    {
        var collection = _liteDatabaseAsync.GetCollection<AppSettings>();

        return collection.Query().SingleOrDefaultAsync();
    }

    public async Task UpdateLocalRootDirectory(string localRootDirectory)
    {
        var collection = _liteDatabaseAsync.GetCollection<AppSettings>();
        var appSettings = await collection.Query().SingleOrDefaultAsync() ?? new AppSettings
        {
            LocalRootDirectory = localRootDirectory
        };

        await collection.UpsertAsync(appSettings);
    }
}