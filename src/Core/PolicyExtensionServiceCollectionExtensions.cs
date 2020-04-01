using System;
using System.Linq;
using AuthorizationExtension.Core;
using AuthorizationExtension.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PolicyExtensionServiceCollectionExtensions
    {
        public static AuthorizationExtensionBuilder AddAuthorizationExtension<TResource, TPermission, TPermissionRole, TPermissionUser>
                                                                  (this IServiceCollection services,
                                                                   Action<AuthorizationExtensionOptions> configure = null)
                                         where TResource : SystemResource
                                         where TPermission : SystemPermission
                                         where TPermissionRole : SystemPermissionRole
                                         where TPermissionUser : SystemPermissionUser
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            services.AddAuthorizationCore();
            services.AddAuthorizationPolicyEvaluator();
            if (configure != null)
            {
                services.Configure(configure);
            }
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMemoryCache();
            services.AddSingleton<IOptions<AuthorizationOptions>, OptionsManager<AuthorizationExtensionOptions>>();
            services.AddScoped<IOptionsSnapshot<AuthorizationOptions>, OptionsManager<AuthorizationExtensionOptions>>();
            services.AddScoped<ISystemResourceService<TResource>, SystemResourceService<TResource>>();
            services.AddScoped<ISystemPermissionService<TPermission>, SystemPermissionService<TPermission,TPermissionRole,TPermissionUser>>();
            services.AddScoped<ISystemPermissionRoleService<TPermissionRole>, SystemPermissionRoleService<TPermissionRole>>();
            services.AddScoped<ISystemPermissionUserService<TPermissionUser>, SystemPermissionUserService<TPermissionUser>>();
            // services.AddScoped<IRoleAccessor, DefaultRoleAccessor>();
            // services.AddScoped<IUserIdAccessor, DefaultUserIdAccessor>();
            services.AddTransient<IPolicyCombiner, PolicyCombiner>();
            services.AddScoped<IResourceIdProvider, ResourceIdProvider>();
            services.AddScoped<IAuthorizeDataProvider, AuthorizeDataProvider<TResource, TPermission>>();
            services.AddScoped<ICacheManager, CacheManager>();
            services.AddScoped<ICache, MsMemoryCache>();
            // services.AddSingleton<IPrincipalAccessor, DefaultPrincipalAccessor>();
            services.AddScoped<IAuthorizeDataManager,AuthorizeDataManager>();
            services.AddScoped<IPermissionMonitor,PermissionMonitor<TResource>>();
            return new AuthorizationExtensionBuilder(typeof(TPermission),typeof(TResource),typeof(TPermissionUser),typeof(TPermissionRole),services);
        }

        public static AuthorizationExtensionBuilder AddAuthorizationExtension<TResource, TPermission>(this IServiceCollection services,
                                                                                           Action<AuthorizationExtensionOptions> configure = null)
                                         where TResource : SystemResource
                                         where TPermission : SystemPermission
            => services.AddAuthorizationExtension<TResource, TPermission, SystemPermissionRole, SystemPermissionUser>(configure);

        public static AuthorizationExtensionBuilder AddAuthorizationExtension<TResource>(this IServiceCollection services,
                                                                              Action<AuthorizationExtensionOptions> configure = null)
                                         where TResource : SystemResource
            => services.AddAuthorizationExtension<TResource, SystemPermission>(configure);

        public static AuthorizationExtensionBuilder AddAuthorizationExtension(this IServiceCollection services,
                                                                   Action<AuthorizationExtensionOptions> configure = null)
            => services.AddAuthorizationExtension<SystemResource>(configure);
        

    }
}