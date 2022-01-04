using System;
using System.Threading.Tasks;
using Google.Apis.Util.Store;
using LiteDB.Async;

namespace MonoDrive.Application.Data
{
    public class LiteDbDataStore : IDataStore 
    {
        public LiteDbDataStore(ILiteDatabaseAsync liteDatabaseAsync)
        {
            Database = liteDatabaseAsync;
        }

        private ILiteDatabaseAsync Database { get; }
        private const string CollectionName = "token";

        public Task StoreAsync<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            var collection = Database.GetCollection<T>(CollectionName);
            return collection.UpsertAsync(GenerateStoredKey(key, typeof(T)), value);
        }

        public Task DeleteAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            var collection = Database.GetCollection<T>(CollectionName);
            return collection.DeleteAsync(GenerateStoredKey(key, typeof(T)));
        }

        public Task<T> GetAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            var collection = Database.GetCollection<T>(CollectionName);
            
            return collection.FindByIdAsync(GenerateStoredKey(key, typeof(T)));
        }

        public Task ClearAsync()
        {
            return Database.DropCollectionAsync(CollectionName);
        }

        private static string GenerateStoredKey(string key, Type t)
        {
            return $"{t.FullName}-{key}";
        }
    }
}