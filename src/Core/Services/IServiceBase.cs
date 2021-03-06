using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationExtension.Core
{
    public interface IServiceBase<TEntity>
    where TEntity:class
    {
        
        Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken);
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken);
        Task<TEntity> FindByIdAsync(string id, CancellationToken cancellationToken);
    }
}