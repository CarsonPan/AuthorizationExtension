using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AuthorizationExtension.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace AuthorizationExtension.Core
{
    public class AuthorizeDataProvider<TResource, TPermission> : IAuthorizeDataProvider
                 where TResource : SystemResource
                 where TPermission : SystemPermission
    {
        protected readonly ISystemResourceService<TResource> SystemResourceService;
        protected readonly ISystemPermissionService<TPermission> SystemPermissionService;


        public AuthorizeDataProvider(ISystemResourceService<TResource> systemResourceService,
                                     ISystemPermissionService<TPermission> systemPermissionService)
        {
            SystemResourceService = systemResourceService;
            SystemPermissionService = systemPermissionService;
        }


        public async Task<AuthorizeData[]> GetAuthorizeDatasAsync(string resourceId, CancellationToken cancellationToken)
        {

            TResource resource = await SystemResourceService.FindByIdAsync(resourceId, cancellationToken);
            if (resource == null)
            {
                throw new InvalidOperationException($" '未定义相关资源：{resourceId}！");
            }
            AuthorizeData authorizeData = new AuthorizeData
            {
                ResoureId = resourceId
            };
            if (string.IsNullOrWhiteSpace(resource.PermissionId))
            {
                throw new InvalidOperationException($"未对ResourceId为：{resourceId}的资源 配置任何权限");
            }
            TPermission permission = await SystemPermissionService.FindByIdAsync(resource.PermissionId, cancellationToken);
            if (permission == null)
            {
                throw new InvalidOperationException($"未对ResourceId为：{resourceId}的资源 配置任何权限");
            }
            authorizeData.AllowedAnonymous = permission.AllowedAnonymous;
            authorizeData.AllowedAllRoles = permission.AllowedAllRoles;    
            authorizeData.DeniedAll = permission.DeniedAll;
            if (!authorizeData.DeniedAll&&!authorizeData.AllowedAllRoles)
            {
                IEnumerable<string>[] result = await Task.WhenAll(SystemPermissionService.GetRolesAsync(permission.Id, cancellationToken),
                                                          SystemPermissionService.GetUsersAsync(permission.Id, cancellationToken));
                authorizeData.AllowedRoles = result[0];     
                authorizeData.AllowedUsers = result[1];
            }

            return new AuthorizeData[] { authorizeData };
        }
    }
}