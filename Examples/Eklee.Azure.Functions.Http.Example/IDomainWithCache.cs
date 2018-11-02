using System.Threading.Tasks;
using Eklee.Azure.Functions.Http.Example.Models;
using Eklee.Azure.Functions.Http.Models;

namespace Eklee.Azure.Functions.Http.Example
{
    public interface IDomainWithCache
    {
        Task<CacheResult<KeyValueDto>> GetAsync(string key);
    }
}
