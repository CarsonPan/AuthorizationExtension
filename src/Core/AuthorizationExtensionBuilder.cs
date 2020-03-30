using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AuthorizationExtension.Core
{
    public class AuthorizationExtensionBuilder
    {
        internal AuthorizationExtensionBuilder(Type permissionType,
                                               Type resourceType,
                                               Type permissionUserType,
                                               Type permissionRoleType,
                                               IServiceCollection services)
        {
            this.PermissionType = permissionType;
            this.ResourceType = resourceType;
            this.PermissionUserType = permissionUserType;
            this.PermissionRoleType = permissionRoleType;
            this.Services = services;

        }
        public Type PermissionType { get; set; }
        public Type ResourceType { get; set; }
        public Type PermissionUserType { get; set; }
        public Type PermissionRoleType { get; set; }
        public IServiceCollection Services { get; set; }

        public AuthorizationExtensionBuilder ReplaceService<TService, TImplementation>()
        {
            return ReplaceService(typeof(TService), typeof(TImplementation));
        }

        public AuthorizationExtensionBuilder ReplaceService(Type serviceType, Type implementationType)
        {
            ServiceDescriptor oldServiceDescriptor = Services.FirstOrDefault(s => s.ServiceType == serviceType && s.ImplementationType == implementationType);
            if (oldServiceDescriptor != null)
            {
                Services.Replace(ServiceDescriptor.Describe(serviceType, implementationType, oldServiceDescriptor.Lifetime));
            }
            return this;
        }
    }
}