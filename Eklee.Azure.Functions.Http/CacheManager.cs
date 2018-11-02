using System;
using System.Threading.Tasks;
using Eklee.Azure.Functions.Http.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Eklee.Azure.Functions.Http
{
    public class CacheManager : ICacheManager
    {
        private readonly IDistributedCache _distributedCache;

        public CacheManager(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<CacheResult<T>> TryGetOrSetIfNotExistAsync<T>(Func<T> getResult, string key, DistributedCacheEntryOptions distributedCacheEntryOptions)
        {
            var value = await _distributedCache.GetStringAsync(key);
            if (value != null)
            {
                return new CacheResult<T>(JsonConvert.DeserializeObject<T>(value), true);
            }

            var result = getResult();

            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(result), distributedCacheEntryOptions);

            return new CacheResult<T>(result, false);
        }
    }
}
