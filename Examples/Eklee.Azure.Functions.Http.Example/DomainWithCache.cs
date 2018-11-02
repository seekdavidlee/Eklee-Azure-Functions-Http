using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eklee.Azure.Functions.Http.Example.Models;
using Eklee.Azure.Functions.Http.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Eklee.Azure.Functions.Http.Example
{
    public class DomainWithCache : IDomainWithCache
    {
        private readonly ICacheManager _cacheManager;
        private static readonly List<KeyValueDto> _repository = new List<KeyValueDto>();
        public DomainWithCache(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;

            if (_repository.Count == 0)
            {
                _repository.Add(new KeyValueDto { Key = "1", Value = "JOHN" });
                _repository.Add(new KeyValueDto { Key = "2", Value = "MARY" });
                _repository.Add(new KeyValueDto { Key = "3", Value = "JANE" });
                _repository.Add(new KeyValueDto { Key = "4", Value = "MIKE" });
                _repository.Add(new KeyValueDto { Key = "5", Value = "RICK" });
                _repository.Add(new KeyValueDto { Key = "6", Value = "ALICE" });
                _repository.Add(new KeyValueDto { Key = "7", Value = "MARCH" });
            }
        }

        public async Task<CacheResult<KeyValueDto>> GetAsync(string key)
        {
            return await _cacheManager.TryGetOrSetIfNotExistAsync(() => _repository.Single(x => x.Key == key), key,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(5)
                });
        }
    }
}
