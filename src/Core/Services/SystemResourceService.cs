using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AuthorizationExtension.Models;
using Microsoft.Extensions.Options;

namespace AuthorizationExtension.Core
{
    public class SystemResourceService<TResource> : ServiceBase<TResource>, ISystemResourceService<TResource>
                 where TResource : SystemResource
    {
        protected readonly IPermissionMonitor PermissionMonitor;
        public SystemResourceService(ISystemResourceStore<TResource> systemResourceStore,
                                     IPermissionMonitor permissionMonitor)
            : base(systemResourceStore)
        {
            PermissionMonitor=permissionMonitor;
        }


        // public async Task<TResource> SetPermissionAsync(string resourceId, TPermission permission, CancellationToken cancellationToken)
        // {
        //     TResource resource = await Store.FindByIdAsync(resourceId, cancellationToken);
        //     resource.PermissionId = permission.Id;
        //     resource = await UpdateAsync(resource, cancellationToken);
        //     await PermissionMonitor.OnResourceChangedAsync(resourceId);
        //     return resource;
        // }

        public override async Task<TResource> UpdateAsync(TResource entity, CancellationToken cancellationToken)
        {
            TResource resource = await Store.FindByIdAsync(entity.Id, cancellationToken);
            if (resource == null)
            {
                throw new Exception($"id为：{entity.Id} 的资源不存在！");
            }
            entity = await Store.UpdateAsync(entity, cancellationToken);
            return entity;
        }

    }
}