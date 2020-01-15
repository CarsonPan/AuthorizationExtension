using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationExtension.Core
{
    public interface IStoreBase<TEntity>
    where TEntity:class
    {
         Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken);
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken);
        Task<TEntity> FindByIdAsync(string id, CancellationToken cancellationToken);
    }
}