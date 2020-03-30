using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuthorizationExtension.Models;
using Microsoft.Extensions.Options;

namespace AuthorizationExtension.Core
{
    public class PermissionMonitor<TResource> : IPermissionMonitor
                 where TResource : SystemResource
    {
        protected readonly bool CacheEnabled;
        protected readonly ICacheManager CacheManager;
        protected readonly ISystemResourceStore<TResource> SystemResourceStore;

        public PermissionMonitor(ISystemResourceStore<TResource> systemResourceStore,
                                 ICacheManager cacheManager,
                                 IOptionsMonitor<AuthorizationExtensionOptions> optionsAccessor)
        {
            
            CacheManager = cacheManager;
            CacheEnabled = (optionsAccessor.CurrentValue?.Cache ?? new CacheOptions()).Enabled;
            SystemResourceStore = systemResourceStore;
        }

        public async Task OnPermissionChangedAsync(string permissionId)
        {
             if (!CacheEnabled)
            {
                return;
            }
            IEnumerable<TResource> resources = await SystemResourceStore.GetResourcesByPermissionId(permissionId,CancellationToken.None);
            if (!resources.IsNullOrEmpty())
            {
                foreach (TResource resource in resources)
                {
                    CacheManager.Remove(resource.Id);
                }
            }
        }

        public async Task OnPermissionChangedAsync(IEnumerable<string> permissionIds)
        {
             if (!CacheEnabled)
            {
                return;
            }
            IEnumerable<TResource> resources = await SystemResourceStore.GetResourcesByPermissionIds(permissionIds, CancellationToken.None);
            if (!resources.IsNullOrEmpty())
            {
                foreach (TResource resource in resources)
                {
                    CacheManager.Remove(resource.Id);
                }
            }
        }

        public Task OnResourceChangedAsync(string resourceId)
        {
             if (CacheEnabled)
            {
                CacheManager.Remove(resourceId);
                
            }
            return Task.CompletedTask;
        }
    }
}