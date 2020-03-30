using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AuthorizationExtension.Models;
using Microsoft.Extensions.Options;

namespace AuthorizationExtension.Core
{
    public class SystemPermissionRoleService<TPermissionRole> : ISystemPermissionRoleService<TPermissionRole>
                 where TPermissionRole : SystemPermissionRole
    {
        protected readonly ISystemPermissionRoleStore<TPermissionRole> SystemPermissionRoleStore;
        protected readonly IPermissionMonitor PermissionMonitor;
        public SystemPermissionRoleService(ISystemPermissionRoleStore<TPermissionRole> systemPermissionRoleStore, IPermissionMonitor permissionMonitor)
        {
            SystemPermissionRoleStore = systemPermissionRoleStore;
            PermissionMonitor = permissionMonitor;
        }

        public async Task<TPermissionRole> CreateAsync(TPermissionRole permissionRole, CancellationToken cancellationToken)
        {
            permissionRole = await SystemPermissionRoleStore.CreateAsync(permissionRole, cancellationToken);
            await PermissionMonitor.OnPermissionChangedAsync(permissionRole.PermissionId);
            return permissionRole;
        }
        public async Task<IEnumerable<TPermissionRole>> CreateAsync(IEnumerable<TPermissionRole> permissionRoles, CancellationToken cancellationToken)
        {
            permissionRoles = await SystemPermissionRoleStore.CreateAsync(permissionRoles, cancellationToken);
            await PermissionMonitor.OnPermissionChangedAsync(permissionRoles.Select(p => p.PermissionId));
            return permissionRoles;
        }

        public async Task<bool> DeleteAsync(string permissionId, string roleId, CancellationToken cancellationToken)
        {
            bool result = await SystemPermissionRoleStore.DeleteAsync(permissionId, roleId, cancellationToken);
            if (result)
            {
                await PermissionMonitor.OnPermissionChangedAsync(permissionId);
            }
            return result;
        }

        public async Task<bool> DeleteAsync(IEnumerable<TPermissionRole> permissionRoles, CancellationToken cancellationToken)
        {
            bool result = await SystemPermissionRoleStore.DeleteAsync(permissionRoles, cancellationToken);
            if (result)
            {
                await PermissionMonitor.OnPermissionChangedAsync(permissionRoles.Select(p=>p.PermissionId));
            }
            return result;
        }

        public  Task<TPermissionRole> FindPermissionRoleAsync(string permissionId, string roleId, CancellationToken cancellationToken)
        {
            return  SystemPermissionRoleStore.FindPermissionRoleAsync(permissionId,roleId,cancellationToken);
        }

        public Task<IEnumerable<TPermissionRole>> GetPermissionRolesByRoleIdAsync(string roleId, CancellationToken cancellationToken)
        {
             return SystemPermissionRoleStore.GetPermissionRolesByRoleIdAsync(roleId,cancellationToken);
        }

        public Task<IEnumerable<TPermissionRole>> GetPermissionRolesByPermissionIdAsync(string permissionId, CancellationToken cancellationToken)
        {
            return SystemPermissionRoleStore.GetPermissionRolesByPermissionIdAsync(permissionId,cancellationToken);
        }
    }
}