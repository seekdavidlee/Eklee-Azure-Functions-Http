namespace Eklee.Azure.Functions.Http.Models
{
    public class CacheResult<T>
    {
        public CacheResult(T result, bool resultIsFromCache)
        {
            Result = result;
            ResultIsFromCache = resultIsFromCache;
        }

        public T Result { get; }
        public bool ResultIsFromCache { get; }
    }
}
