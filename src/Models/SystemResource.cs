using System.Collections.Generic;

namespace AuthorizationExtension.Models
{
    public class SystemResource
    {
        public string Id{get;set;}
        public ResourceType ResourceType{get;set;}

        public string PermissionId{get;set;}
        public bool AllowAnonymous{get;set;}
        public bool AllowAllRoles{get;set;}
        public string ParentId{get;set;}
        public string Description{get;set;}
        public bool IsEnabled{get;set;}
    }
}