using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AuthorizationExtension.Models;

namespace AuthorizationExtension.Core
{
    public interface ISystemPermissionUserStore<TPermissionUser>
                     where TPermissionUser:SystemPermissionUser
    {
         Task<TPermissionUser> CreateAsync(TPermissionUser permissionUser,CancellationToken cancellationToken);
         Task<IEnumerable<TPermissionUser>> CreateAsync(IEnumerable<TPermissionUser> permissionUsers,CancellationToken cancellationToken);
         Task<bool> DeleteAsync(string permissionId,string userId, CancellationToken cancellationToken);
         Task<bool> DeleteAsync(IEnumerable<TPermissionUser> permissionUsers, CancellationToken cancellationToken);
         Task<TPermissionUser> FindPermissionUserAsync(string permissionId,string userId, CancellationToken cancellationToken);
         Task<IEnumerable<TPermissionUser>> GetPermissionUsersByUserId(string userId, CancellationToken cancellationToken);
         Task<IEnumerable<TPermissionUser>> GetPermissionUsersByPermissionId(string permissionId, CancellationToken cancellationToken);
    }
}