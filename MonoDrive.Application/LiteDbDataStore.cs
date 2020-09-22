using System;
using System.Threading.Tasks;
using Google.Apis.Json;
using Google.Apis.Util.Store;
using LiteDB;

namespace MonoDrive.Application
{
    public class LiteDbDataStore : IDataStore
    {
        public LiteDbDataStore(string connectionString)
        {
            Database = new LiteDatabase(connectionString);
        }

        private LiteDatabase Database { get; }
        private const string CollectionName = "token";

        public Task StoreAsync<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            var collection = Database.GetCollection<T>(CollectionName);
            collection.Insert(value);

            return Task.FromResult(0);
        }

        public Task DeleteAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            var collection = Database.GetCollection<T>(CollectionName);
            // collection.Find()
            // collection.Delete(value);

            return Task.FromResult(0);
        }

        public Task<T> GetAsync<T>(string key)
        {
            throw new System.NotImplementedException();
        }

        public Task ClearAsync()
        {
            throw new System.NotImplementedException();
        }

        private static string GenerateStoredKey(string key, Type t)
        {
            return $"{t.FullName}-{key}";
        }
    }
}