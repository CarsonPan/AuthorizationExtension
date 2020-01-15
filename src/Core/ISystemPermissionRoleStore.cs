using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AuthorizationExtension.Models;

namespace AuthorizationExtension.Core
{
    public interface ISystemPermissionRoleStore
    {
         Task AddToRoleAsync(SystemPermission permission,string roleId,CancellationToken cancellationToken);
         Task RemoveFromRoleAsync(SystemPermission permission, string roleId, CancellationToken cancellationToken);
         Task<IList<string>> GetRolesAsync(SystemPermission permission, CancellationToken cancellationToken);
         Task<bool> IsInRoleAsync(SystemPermission permission, string roleId, CancellationToken cancellationToken);
         Task<IList<SystemPermission>> GetPermissionInRoleAsync(string roleId, CancellationToken cancellationToken);
    }
}