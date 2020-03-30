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
    public class SystemPermissionUserService<TPermissionUser>:ISystemPermissionUserService<TPermissionUser>
                 where TPermissionUser:SystemPermissionUser
    {
        protected readonly ISystemPermissionUserStore<TPermissionUser> SystemPermissionUserStore;
        protected readonly IPermissionMonitor PermissionMonitor;
        public SystemPermissionUserService(ISystemPermissionUserStore<TPermissionUser> systemPermissionUserStore,
                                           IPermissionMonitor permissionMonitor)
        {
            PermissionMonitor=permissionMonitor;
            SystemPermissionUserStore = systemPermissionUserStore;
        }


        

        public async Task<TPermissionUser> CreateAsync(TPermissionUser permissionUser, CancellationToken cancellationToken)
        {
             permissionUser=await SystemPermissionUserStore.CreateAsync(permissionUser,cancellationToken);
            await PermissionMonitor.OnPermissionChangedAsync(permissionUser.PermissionId);
            return permissionUser;
        }

        public async Task<IEnumerable<TPermissionUser>> CreateAsync(IEnumerable<TPermissionUser> permissionUsers, CancellationToken cancellationToken)
        {
            permissionUsers=await SystemPermissionUserStore.CreateAsync(permissionUsers,cancellationToken);
            await PermissionMonitor.OnPermissionChangedAsync(permissionUsers.Select(p=>p.PermissionId));
            return permissionUsers;
        }

        public async Task<bool> DeleteAsync(string permissionId,string userId, CancellationToken cancellationToken)
        {
            bool result=await SystemPermissionUserStore.DeleteAsync(permissionId,userId,cancellationToken);
            if(result)
            {
                await PermissionMonitor.OnPermissionChangedAsync(permissionId);
            }
            return result;
        }

        public async Task<bool> DeleteAsync(IEnumerable<TPermissionUser> permissionUsers, CancellationToken cancellationToken)
        {
            bool result=await SystemPermissionUserStore.DeleteAsync(permissionUsers,cancellationToken);
            if(result)
            {
                await PermissionMonitor.OnPermissionChangedAsync(permissionUsers.Select(p=>p.PermissionId));
            }
            return result;
        }

        public Task<TPermissionUser> FindPermissionUserAsync(string permissionId, string userId, CancellationToken cancellationToken)
        {
            return SystemPermissionUserStore.FindPermissionUserAsync(permissionId,userId,cancellationToken);
        }
        public Task<IEnumerable<TPermissionUser>> GetPermissionUsersByPermissionId(string permissionId, CancellationToken cancellationToken)
        {
            return SystemPermissionUserStore.GetPermissionUsersByPermissionId(permissionId,cancellationToken);
        }

        public Task<IEnumerable<TPermissionUser>> GetPermissionUsersByUserId(string userId, CancellationToken cancellationToken)
        {
            return SystemPermissionUserStore.GetPermissionUsersByUserId(userId,cancellationToken);
        }

    }
}