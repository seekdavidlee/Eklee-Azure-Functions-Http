using System;
using System.Threading.Tasks;
using Eklee.Azure.Functions.Http.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Eklee.Azure.Functions.Http
{
    public interface ICacheManager
    {
        Task<CacheResult<T>> TryGetOrSetIfNotExistAsync<T>(Func<T> getResult, string key, DistributedCacheEntryOptions distributedCacheEntryOptions);
    }
}
