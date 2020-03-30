using System;
using Microsoft.Extensions.Caching.Memory;

namespace AuthorizationExtension.Core
{
    public class MsMemoryCache : ICache
    {
        protected readonly IMemoryCache MemoryCache;

        public MsMemoryCache(IMemoryCache cache)
        {
            MemoryCache=cache;
        }
        public void Remove(string key)
        {
            MemoryCache.Remove(key);
        }

        public void Set<T>(string key, T value, ExpirationMode expirationMode, int expiration)
        {
            MemoryCacheEntryOptions options=new MemoryCacheEntryOptions();
            TimeSpan expire=TimeSpan.FromSeconds(expiration);
            if(expirationMode==ExpirationMode.Sliding)
            {
                options.SlidingExpiration=expire;
            }
            else
            {
                options.AbsoluteExpirationRelativeToNow=expire;
            }
            MemoryCache.Set(key,value,options);
        }

        public bool TryGetValue<T>(string key, out T value)
        {
           return MemoryCache.TryGetValue(key,out value);
        }
    }
}