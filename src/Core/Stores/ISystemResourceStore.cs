using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AuthorizationExtension.Models;

namespace AuthorizationExtension.Core
{
    public interface ISystemResourceStore<TResource>:IStoreBase<TResource> where TResource:SystemResource
    {
        Task<IEnumerable<TResource>> GetResourcesByPermissionId(string permissionId,CancellationToken cancellationToken);
        Task<IEnumerable<TResource>> GetResourcesByPermissionIds(IEnumerable<string> permissionIds,CancellationToken cancellationToken);
    }
}