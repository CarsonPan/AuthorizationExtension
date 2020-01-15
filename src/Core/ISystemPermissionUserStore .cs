using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AuthorizationExtension.Models;

namespace AuthorizationExtension.Core
{
    public interface ISystemPermissionUserStore 
    {
         Task AddToUserAsync(SystemPermission permission,string userId,CancellationToken cancellationToken);
         Task RemoveFromUserAsync(SystemPermission permission, string userId, CancellationToken cancellationToken);
         Task<IList<string>> GetUsersAsync(SystemPermission permission, CancellationToken cancellationToken);
         Task<bool> IsInUserAsync(SystemPermission permission, string userId, CancellationToken cancellationToken);
         Task<IList<SystemPermission>> GetPermissionInUserAsync(string userId, CancellationToken cancellationToken);
    }
}