using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuthorizationExtension.Models;
using Microsoft.Extensions.Options;

namespace AuthorizationExtension.Core
{
    public class SystemPermissionService<TPermission, TPermissionRole, TPermissionUser> : ServiceBase<TPermission>, ISystemPermissionService<TPermission>
                 where TPermission : SystemPermission
                 where TPermissionRole : SystemPermissionRole
                 where TPermissionUser : SystemPermissionUser
    {
        protected readonly ISystemPermissionRoleStore<TPermissionRole> SystemPermissionRoleStore;
        protected readonly ISystemPermissionUserStore<TPermissionUser> SystemPermissionUserStore;
        protected readonly IPermissionMonitor PermissionMonitor;
        public SystemPermissionService(ISystemPermissionStore<TPermission> store,
                                       ISystemPermissionRoleStore<TPermissionRole> systemPermissionRoleStore,
                                       ISystemPermissionUserStore<TPermissionUser> systemPermissionUserStore,
                                       IPermissionMonitor permissionMonitor) :
                 base(store)
        {
            SystemPermissionRoleStore = systemPermissionRoleStore;
            SystemPermissionUserStore = systemPermissionUserStore;
            PermissionMonitor=permissionMonitor;
        }

        public async Task<IEnumerable<string>> GetRolesAsync(string permissionId, CancellationToken cancellationToken)
        {
            IEnumerable<TPermissionRole> permissionRoles= await SystemPermissionRoleStore.GetPermissionRolesByPermissionIdAsync(permissionId, cancellationToken);
            return permissionRoles.Select(p=>p.RoleId);
        }

        public async Task<IEnumerable<string>> GetUsersAsync(string permissionId, CancellationToken cancellationToken)
        {
            IEnumerable<TPermissionUser> permissionUsers= await SystemPermissionUserStore.GetPermissionUsersByPermissionId(permissionId, cancellationToken);
            return permissionUsers.Select(p=>p.UserId);
        }

      

        public override async  Task<TPermission> UpdateAsync(TPermission entity, CancellationToken cancellationToken)
        {
            TPermission permission = await Store.FindByIdAsync(entity.Id, cancellationToken);
            if (permission == null)
            {
                throw new Exception($"id为：{entity.Id} 的权限设置不存在！");
            }
            entity=await base.UpdateAsync(entity,cancellationToken);
            if(entity.AllowedAllRoles!=permission.AllowedAllRoles||
               entity.AllowedAnonymous!=permission.AllowedAnonymous||
               entity.DeniedAll!=permission.DeniedAll)
            {
                await PermissionMonitor.OnPermissionChangedAsync(entity.Id);
            }
            return entity;
        }

    }
}