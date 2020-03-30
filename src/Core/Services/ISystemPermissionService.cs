
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AuthorizationExtension.Models;

namespace AuthorizationExtension.Core
{
    public interface ISystemPermissionService<TPermission> :IServiceBase<TPermission>
                     where TPermission:SystemPermission
    {
        Task<IEnumerable<string>> GetRolesAsync(string permissionId, CancellationToken cancellationToken);
        Task<IEnumerable<string>> GetUsersAsync(string permissionId, CancellationToken cancellationToken);
    }
}