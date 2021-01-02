using System;
using System.Threading.Tasks;
using Google.Apis.Util.Store;
using LiteDB;

namespace MonoDrive.Application.Data
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
            collection.Upsert(GenerateStoredKey(key, typeof(T)), value);

            return Task.CompletedTask;
        }

        public Task DeleteAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            var collection = Database.GetCollection<T>(CollectionName);
            collection.Delete(GenerateStoredKey(key, typeof(T)));

            return Task.CompletedTask;
        }

        public Task<T> GetAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }
            
            var taskCompletionSource = new TaskCompletionSource<T>();
            
            var collection = Database.GetCollection<T>(CollectionName);
            
            try
            {
                var value = collection.FindById(GenerateStoredKey(key, typeof(T)));
                taskCompletionSource.SetResult(value);
            }
            catch (Exception ex)
            {
                taskCompletionSource.SetException(ex);
            }
            
            return taskCompletionSource.Task;
        }

        public Task ClearAsync()
        {
            Database.DropCollection(CollectionName);

            return Task.CompletedTask;
        }

        private static string GenerateStoredKey(string key, Type t)
        {
            return $"{t.FullName}-{key}";
        }
    }
}