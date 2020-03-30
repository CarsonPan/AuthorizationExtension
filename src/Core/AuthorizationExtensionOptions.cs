using Microsoft.AspNetCore.Authorization;

namespace AuthorizationExtension.Core
{
    public class AuthorizationExtensionOptions:AuthorizationOptions
    {
        public CacheOptions Cache{get;set;}
        public ResourceOptions Resource{get;set;}
    }
}