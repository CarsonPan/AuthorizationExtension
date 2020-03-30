using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace AuthorizationExtension.Core
{
    public class AuthorizeDataManager:IAuthorizeDataManager
    {
        protected readonly IResourceIdProvider ResourceIdProvider;
        protected readonly IAuthorizeDataProvider AuthorizeDataProvider;
        protected readonly ICacheManager CacheManager;
        protected readonly bool CacheEnabled;
        public AuthorizeDataManager(IResourceIdProvider resourceIdProvider,
                                    IAuthorizeDataProvider authorizeDataProvider,
                                    ICacheManager cacheManager,
                                    IOptionsMonitor<AuthorizationExtensionOptions> options)
        {
            ResourceIdProvider = resourceIdProvider;
            AuthorizeDataProvider = authorizeDataProvider;

            CacheManager = cacheManager;
            CacheEnabled = (options.CurrentValue?.Cache ?? new CacheOptions()).Enabled;
        }

        /// <summary>
        /// 将多个权限数据合并为一个
        /// </summary>
        /// <param name="authorizeDatas"></param>
        /// <returns></returns>
        private AuthorizeData Combine(AuthorizeData[] authorizeDatas)
        {
            if (authorizeDatas == null || authorizeDatas.Length == 0)
            {
                return null;
            }
            if (authorizeDatas.Length == 1)
            {
                return authorizeDatas[0];
            }
            List<string> authenticationSchemes = new List<string>();
            List<string> allowedRoles = new List<string>();
            List<string> allowedUsers = new List<string>();
            List<string> policies = new List<string>();
            for (int i = 0; i < authorizeDatas.Length; i++)
            {
                //只要有一个拒绝所有那么整个拒绝
                authorizeDatas[0].DeniedAll |= authorizeDatas[i].DeniedAll;
                //所有权限设置都允许匿名访问才允许匿名访问
                authorizeDatas[0].AllowedAnonymous &= authorizeDatas[i].AllowedAnonymous;
                //所有权限设置都允许登录即可访问才允许访问
                authorizeDatas[0].AllowedAllRoles &= authorizeDatas[i].AllowedAllRoles;
                authenticationSchemes.Append(authorizeDatas[i].AuthenticationSchemes);
                allowedRoles.Append(authorizeDatas[i].AllowedRoles);
                allowedUsers.Append(authorizeDatas[i].AllowedUsers);
                policies.Append(authorizeDatas[i].Policies);
            }
            authorizeDatas[0].AuthenticationSchemes=authenticationSchemes.Distinct();
            authorizeDatas[0].AllowedRoles=allowedRoles.Distinct();
            authorizeDatas[0].AllowedUsers=allowedUsers.Distinct();
            authorizeDatas[0].Policies=policies.Distinct();
            return authorizeDatas[0];
        }
        public async Task<AuthorizeData> GetAuthorizeDataAsync()
        {
            string resourceId = ResourceIdProvider.GetResourceId();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            if (CacheEnabled)
            {
                return await CacheManager.GetOrCreateAsync<AuthorizeData>(resourceId,()=>GetAuthorizeDataCoreAsync(resourceId,cancellationTokenSource.Token));
            }
            else
            {
                return await GetAuthorizeDataCoreAsync(resourceId, cancellationTokenSource.Token);
            }

        }

        private async Task<AuthorizeData> GetAuthorizeDataCoreAsync(string resourceId,CancellationToken cancellationToken)
        {
            AuthorizeData[] authorizeDatas= await AuthorizeDataProvider.GetAuthorizeDatasAsync(resourceId,cancellationToken);
            return Combine(authorizeDatas);
        }
    }
}