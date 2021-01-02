using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        
        [Theory]
        [InlineData("token")]
        public async Task PrintCollectionByName(string name)
        {
            await PrintCollectionSnapshot(name);
        }

        [Fact]
        public async Task PrintLocalDirectoriesInfo()
        {
            await PrintCollectionSnapshot<LocalDirectoryInfo>();
        }

        private async Task PrintCollectionSnapshot<T>()
        {
            var collection = _dbFixture.Database.GetCollection<T>();

            var array = collection.FindAll().ToArray();
            var count = array.Length;

            _testOutputHelper.WriteLine($"Collection name: {collection.Name}");

            if (count <= 0)
            {
                _testOutputHelper.WriteLine("There is no data.");
                return;
            }

            _testOutputHelper.WriteLine($"{count} row(s) found.");

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            Directory.CreateDirectory("snapshots");

            using var fileStream = File.Create($"snapshots/{collection.Name}_{DateTimeOffset.Now.ToUnixTimeSeconds()}.json");
            await JsonSerializer.SerializeAsync(fileStream, array, options);

            _testOutputHelper.WriteLine($"Snapshot file: {fileStream.Name}");
        }

        private async Task PrintCollectionSnapshot(string name)
        {
            var collection = _dbFixture.Database.GetCollection(name);

            var bsonDocuments = collection.FindAll().ToArray();
            var count = bsonDocuments.Length;

            _testOutputHelper.WriteLine($"Collection name: {collection.Name}");

            if (count <= 0)
            {
                _testOutputHelper.WriteLine("There is no data.");
                return;
            }

            _testOutputHelper.WriteLine($"{count} row(s) found.");

            Directory.CreateDirectory("snapshots");

            var fileName = Path.GetFullPath($"snapshots/{collection.Name}_{DateTimeOffset.Now.ToUnixTimeSeconds()}.json");
            //_dbFixture.Database.Mapper.Deserialize<Dictionary<string, object>>(bsonDocuments);
            
            var jsonBuilder = new StringBuilder();

            foreach (var document in bsonDocuments)
            {
                LiteDB.JsonSerializer.Serialize(document, jsonBuilder);
            }

            await File.WriteAllTextAsync(fileName, jsonBuilder.ToString());

            _testOutputHelper.WriteLine($"Snapshot file: {fileName}");
        }
    }
}