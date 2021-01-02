using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MonoDrive.Application.Models;
using Xunit;
using Xunit.Abstractions;

namespace MonoDrive.Application.Test
{
    public class LiteDbTest : IClassFixture<LiteDbFixture>
    {
        private readonly LiteDbFixture _dbFixture;
        private readonly ITestOutputHelper _testOutputHelper;

        public LiteDbTest(LiteDbFixture dbFixture, ITestOutputHelper testOutputHelper)
        {
            _dbFixture = dbFixture;
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task PrintLocalDirectoriesInfo()
        {
            await PrintCollectionSnapshot<LocalDirectoryInfo>();
        }

        private async Task PrintCollectionSnapshot<T>()
        {
            var collection = _dbFixture.Database.GetCollection<T>();

            var directoriesInfo = collection.FindAll().ToArray();
            var count = directoriesInfo.Length;

            _testOutputHelper.WriteLine($"Collection name: {collection.Name}");

            if (count <= 0)
            {
                _testOutputHelper.WriteLine("There is no data.");
                return;
            }

            _testOutputHelper.WriteLine($"{count} rows found.");

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            Directory.CreateDirectory("snapshots");

            using var fileStream = File.Create($"snapshots/{collection.Name}_{DateTimeOffset.Now.ToUnixTimeSeconds()}.json");
            await JsonSerializer.SerializeAsync(fileStream, directoriesInfo, options);

            _testOutputHelper.WriteLine($"Snapshot file: {fileStream.Name}");
        }
    }
}