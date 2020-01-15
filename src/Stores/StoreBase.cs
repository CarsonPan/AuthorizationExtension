using System;
using System.Threading;
using System.Threading.Tasks;
using AuthorizationExtension.Core;
using AuthorizationExtension.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationExtension.Stores
{
    public abstract class StoreBase<TEntity, TDbContext> : IStoreBase<TEntity>
    where TEntity : class
    where TDbContext : DbContext
    {
        protected readonly TDbContext DbContext;
        protected readonly DbSet<TEntity> Table;
        private bool _disposed;
        public StoreBase(TDbContext dbContext)
        {
            DbContext = dbContext;
            Table=DbContext.Set<TEntity>();
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
        public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            await Table.AddAsync(entity,cancellationToken);
            await DbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();
             Table.Remove(entity);
            int count=await DbContext.SaveChangesAsync();
            return count>0;
        }

        public  Task<TEntity> FindByIdAsync(string id, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
            return Table.FindAsync(id,cancellationToken).AsTask();
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();
             cancellationToken.ThrowIfCancellationRequested();
            Table.Update(entity);
            await DbContext.SaveChangesAsync();
            return entity;
        }
    }
}