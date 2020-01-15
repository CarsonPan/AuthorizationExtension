using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuthorizationExtension.Core;
using AuthorizationExtension.Data;
using AuthorizationExtension.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationExtension.Stores
{
    public class SystemPermissionRoleStore<TRole> : ISystemPermissionRoleStore,IDisposable
     where TRole : class
    {
        protected readonly AuthorizationDbContext DbContext;
        protected readonly DbSet<SystemPermissionRole> Table;
        protected readonly IRoleStore<TRole> RoleStore;
        private bool _disposed;
        public SystemPermissionRoleStore(AuthorizationDbContext dbContext,IRoleStore<TRole> roleStore)
        {
            DbContext = dbContext;
            Table=DbContext.SystemPermissionRoles;
            RoleStore=roleStore;
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                DbContext.Dispose();
                _disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        
        public async Task AddToRoleAsync(SystemPermission permission, string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (permission == null)
            {
                throw new ArgumentNullException(nameof(permission));
            }
            if (string.IsNullOrWhiteSpace(roleId))
            {
                throw new ArgumentNullException(nameof(roleId));
            }
            var roleEntity = await RoleStore.FindByIdAsync(roleId, cancellationToken);
            if (roleEntity == null)
            {
                throw new InvalidOperationException($"Role {roleId} does not exit.");
            }
            SystemPermissionRole permissionRole=new SystemPermissionRole()
            {
                PermissionId=permission.Id,
                RoleId=roleId
            };
            await Table.AddAsync(permissionRole);
            DbContext.SaveChanges();
        }

        public async Task<IList<SystemPermission>> GetPermissionInRoleAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var permissionRoles= Table.Where(pr=>pr.RoleId==roleId)
                                      .Join(DbContext.SystemPermissions,pr=>pr.PermissionId,p=>p.Id, (pr,p)=>p);
            return await permissionRoles.ToListAsync();
        }

        public async Task<IList<string>> GetRolesAsync(SystemPermission permission, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var roles=Table.Where(pr=>pr.PermissionId==permission.Id).Select(pr=>pr.RoleId);
            return await roles.ToListAsync();
        }

        public async Task<bool> IsInRoleAsync(SystemPermission permission, string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (permission == null)
            {
                throw new ArgumentNullException(nameof(permission));
            }
            if (string.IsNullOrWhiteSpace(roleId))
            {
                throw new ArgumentNullException(nameof(roleId));
            }
            var permissionRole = await FindPermissionRoleAsync(permission.Id, roleId, cancellationToken);
            return permissionRole != null;
           
        }

        protected Task<SystemPermissionRole>  FindPermissionRoleAsync(string permissionId,string roleId,CancellationToken cancellationToken)
        {
            return Table.FindAsync(new object[]{permissionId,roleId},cancellationToken).AsTask();
        }

        public async Task RemoveFromRoleAsync(SystemPermission permission, string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            SystemPermissionRole permissionRole=await FindPermissionRoleAsync(permission.Id,roleId,cancellationToken);
            if(permissionRole!=null)
            {
                Table.Remove(permissionRole);
                DbContext.SaveChanges();
            }
        }
    }
}