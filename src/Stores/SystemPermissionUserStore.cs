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
    public class SystemPermissionUserStore<TUser> : ISystemPermissionUserStore, IDisposable
    where TUser : class
    {
        protected readonly AuthorizationDbContext DbContext;
        protected readonly DbSet<SystemPermissionUser> Table;
        protected readonly IUserStore<TUser> UserStore;
        private bool _disposed;

        public SystemPermissionUserStore(AuthorizationDbContext dbContext,IUserStore<TUser> userStore)
        {
            DbContext=dbContext;
            Table=dbContext.SystemPermissionUsers;
            UserStore=userStore;
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

        public async Task AddToUserAsync(SystemPermission permission, string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (permission == null)
            {
                throw new ArgumentNullException(nameof(permission));
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }
            var userEntity = await UserStore.FindByIdAsync(userId, cancellationToken);
            if (userEntity == null)
            {
                throw new InvalidOperationException($"User {userId} does not exit.");
            }
            SystemPermissionUser permissionUser=new  SystemPermissionUser()
            {
                PermissionId=permission.Id,
                UserId=userId
            };
            await Table.AddAsync(permissionUser);
            DbContext.SaveChanges();
        }

        public async Task<IList<SystemPermission>> GetPermissionInUserAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var permissionRoles= Table.Where(pr=>pr.UserId==userId)
                                      .Join(DbContext.SystemPermissions,pr=>pr.PermissionId,p=>p.Id, (pr,p)=>p);
            return await permissionRoles.ToListAsync();
        }

        public async Task<IList<string>> GetUsersAsync(SystemPermission permission, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var users=Table.Where(pr=>pr.PermissionId==permission.Id).Select(pr=>pr.UserId);
            return await users.ToListAsync();
        }

        public async Task<bool> IsInUserAsync(SystemPermission permission, string userId, CancellationToken cancellationToken)
        {
             cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (permission == null)
            {
                throw new ArgumentNullException(nameof(permission));
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }
            var permissionUser = await FindPermissionUserAsync(permission.Id, userId, cancellationToken);
            return permissionUser != null;
        }

        protected Task<SystemPermissionUser>  FindPermissionUserAsync(string permissionId,string userId,CancellationToken cancellationToken)
        {
            return Table.FindAsync(new object[]{permissionId,userId},cancellationToken).AsTask();
        }

        public async Task RemoveFromUserAsync(SystemPermission permission, string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            SystemPermissionUser permissionUser=await FindPermissionUserAsync(permission.Id,userId,cancellationToken);
            if(permissionUser!=null)
            {
                Table.Remove(permissionUser);
                DbContext.SaveChanges();
            }
        }
    }
}