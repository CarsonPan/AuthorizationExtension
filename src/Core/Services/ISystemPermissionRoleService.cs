using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AuthorizationExtension.Models;

namespace AuthorizationExtension.Core
{
    public interface ISystemPermissionRoleService<TPermissionRole>
                     where TPermissionRole:SystemPermissionRole
    {
        Task<TPermissionRole> CreateAsync(TPermissionRole permissionRole,CancellationToken cancellationToken);
         Task<IEnumerable<TPermissionRole>> CreateAsync(IEnumerable<TPermissionRole> permissionRoles,CancellationToken cancellationToken);
         Task<bool> DeleteAsync(string permissionId, string roleId, CancellationToken cancellationToken);
         Task<bool> DeleteAsync(IEnumerable<TPermissionRole> permissionRoles, CancellationToken cancellationToken);
         Task<TPermissionRole> FindPermissionRoleAsync(string permissionId,string roleId, CancellationToken cancellationToken);
         Task<IEnumerable<TPermissionRole>> GetPermissionRolesByRoleIdAsync(string roleId, CancellationToken cancellationToken);
         Task<IEnumerable<TPermissionRole>> GetPermissionRolesByPermissionIdAsync(string permissionId, CancellationToken cancellationToken);
    }
}