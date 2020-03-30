using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace AuthorizationExtension.Core
{
    public abstract class ServiceBase<TEntity> : IServiceBase<TEntity>
    where TEntity : class
    {
     
        protected readonly IStoreBase<TEntity> Store;

        protected ServiceBase(IStoreBase<TEntity> store)
        {
            Store=store;
        }

        
        public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            entity = await Store.CreateAsync(entity, cancellationToken);
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            bool result = await Store.DeleteAsync(entity, cancellationToken);
            return result;
        }

        public virtual Task<TEntity> FindByIdAsync(string id, CancellationToken cancellationToken)
        {
            return Store.FindByIdAsync(id, cancellationToken);
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            entity = await Store.UpdateAsync(entity, cancellationToken);
            return entity;
        }
    }
}