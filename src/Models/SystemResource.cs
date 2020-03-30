using System.Collections.Generic;

namespace AuthorizationExtension.Models
{
    public class SystemResource
    {
        public string Id{get;set;}
        public string Name{get;set;}
        public ResourceType ResourceType{get;set;}

        public string PermissionId{get;set;}  
        public string ParentId{get;set;}
        public string Description{get;set;}
    }
}