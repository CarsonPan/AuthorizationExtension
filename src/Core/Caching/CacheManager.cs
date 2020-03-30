using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace AuthorizationExtension.Core
{
    public class CacheManager :ICacheManager
    {
        protected readonly ICache Cache;
        protected readonly CacheOptions CacheOptions;
        public CacheManager(ICache cache,IOptionsMonitor<AuthorizationExtensionOptions> optionsAccessor)
        {
            Cache=cache;
            CacheOptions=optionsAccessor.CurrentValue?.Cache??new CacheOptions();
        }
        private string GetKey(string key,string region=null)
        {
            if(string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            return $@"{CacheConsts.CachePrefix}.{region}.{key}";
        } 
        public async Task<TResult> GetOrCreateAsync<TResult>(string key, Func<string, Task<TResult>> factory)
        {
            TResult result;
            if(!TryGetValue(key,out result))
            {
                if(factory==null)
                {
                    throw new ArgumentNullException(nameof(factory));
                }
                result= await factory(key);
                Set(key,result);
            }
            return result;
        }

        public async Task<TResult> GetOrCreateAsync<T, TResult>(string key, T arg, Func<string, T, Task<TResult>> factory)
        {
            TResult result;
            if(!TryGetValue(key,out result))
            {
                if(factory==null)
                {
                    throw new ArgumentNullException(nameof(factory));
                }
                result= await factory(key,arg);
                Set(key,result);
            }
            return result;
        }

        public void Remove(string key)
        {
            Cache.Remove(GetKey(key));
        }

        public void Set<T>(string key, T value)
        {
            Cache.Set(GetKey(key),value,CacheOptions.ExpirationMode,CacheOptions.Expiration);
        }

        public bool TryGetValue<TResult>(string key, out TResult value)
        {
            return Cache.TryGetValue(GetKey(key),out value);
        }

        public bool TryGetValue<TResult>(string key, string region, out TResult value)
        {
            return Cache.TryGetValue(GetKey(key,region),out value);
        }

        public void Set<T>(string key, string region, T value)
        {
            Cache.Set(GetKey(key,region),value,CacheOptions.ExpirationMode,CacheOptions.Expiration);
        }

        public void Remove(string key, string region)
        {
            Cache.Remove(GetKey(key,region));
        }

       

       

        public async Task<TResult> GetOrCreateAsync<T, TResult>(string key, T arg0, CancellationToken cancellationToken, Func<T, CancellationToken, Task<TResult>> factory)
        {
            TResult result;
            if(!TryGetValue(key,out result))
            {
                if(factory==null)
                {
                    throw new ArgumentNullException(nameof(factory));
                }
                result=await factory(arg0,cancellationToken);
                Set(key,result);
            }
            return result;
        }

        public async Task<TResult> GetOrCreateAsync<T0, T1, TResult>(string key, T0 arg0, T1 arg1, CancellationToken cancellationToken, Func<T0, T1, CancellationToken, Task<TResult>> factory)
        {
            TResult result;
            if(!TryGetValue(key,out result))
            {
                if(factory==null)
                {
                    throw new ArgumentNullException(nameof(factory));
                }
                result=await factory(arg0,arg1,cancellationToken);
                Set(key,result);
            }
            return result;
        }

        public async Task<TResult> GetOrCreateAsync<T, TResult>(string key, string region, T arg0, CancellationToken cancellationToken, Func<T, CancellationToken, Task<TResult>> factory)
        {
            TResult result;
            if(!TryGetValue(key,out result))
            {
                if(factory==null)
                {
                    throw new ArgumentNullException(nameof(factory));
                }
                result=await factory(arg0,cancellationToken);
                Set(key,region,result);
            }
            return result;
        }

        public async Task<TResult> GetOrCreateAsync<T0, T1, TResult>(string key, string region, T0 arg0, T1 arg1, CancellationToken cancellationToken, Func<T0, T1, CancellationToken, Task<TResult>> factory)
        {
            TResult result;
            if(!TryGetValue(key,out result))
            {
                if(factory==null)
                {
                    throw new ArgumentNullException(nameof(factory));
                }
                result=await factory(arg0,arg1,cancellationToken);
                Set(key,region,result);
            }
            return result;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory)
        {
            T result;
            if(!TryGetValue(key,out result))
            {
                if(factory==null)
                {
                    throw new ArgumentNullException(nameof(factory));
                }
                result=await factory();
                Set(key,result);
            }
            return result;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, string region, Func<Task<T>> factory)
        {
             T result;
            if(!TryGetValue(key,out result))
            {
                if(factory==null)
                {
                    throw new ArgumentNullException(nameof(factory));
                }
                result=await factory();
                Set(key,region,result);
            }
            return result;
        }
    }
}