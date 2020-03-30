using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationExtension.Core
{
    public interface ICacheManager
    {
         bool TryGetValue<TResult>(string key,out TResult value);
        void Set<T>(string key,T value);
        void Remove(string key);

        Task<T> GetOrCreateAsync<T>(string key,Func<Task<T>> factory);
        Task<TResult> GetOrCreateAsync<T,TResult>(string key,T arg0,CancellationToken cancellationToken,Func<T,CancellationToken,Task<TResult>> factory);
        Task<TResult> GetOrCreateAsync<T0,T1,TResult>(string key, T0 arg0,T1 arg1,CancellationToken cancellationToken,Func<T0,T1,CancellationToken,Task<TResult>> factory);

        bool TryGetValue<TResult>(string key,string region,out TResult value);
        void Set<T>(string key,string region,T value);
        void Remove(string key,string region);
        Task<T> GetOrCreateAsync<T>(string key,string region,Func<Task<T>> factory);
        Task<TResult> GetOrCreateAsync<T,TResult>(string key ,string region,T arg0,CancellationToken cancellationToken,Func<T,CancellationToken,Task<TResult>> factory);
        Task<TResult> GetOrCreateAsync<T0,T1,TResult>(string key,string region, T0 arg0,T1 arg1,CancellationToken cancellationToken,Func<T0,T1,CancellationToken,Task<TResult>> factory);
    }
}