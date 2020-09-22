using System.Threading.Tasks;
using Google.Apis.Util.Store;

namespace MonoDrive.Application
{
    public class LiteDbDataStore : IDataStore
    {
        public Task StoreAsync<T>(string key, T value)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAsync<T>(string key)
        {
            throw new System.NotImplementedException();
        }

        public Task<T> GetAsync<T>(string key)
        {
            throw new System.NotImplementedException();
        }

        public Task ClearAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}