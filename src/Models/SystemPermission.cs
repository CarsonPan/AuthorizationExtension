namespace AuthorizationExtension.Models
{
    public class SystemPermission
    {
        public string Id{get;set;}
        public string Name{get;set;}
        public string  Description{get;set;}
        public bool AllowedAnonymous{get;set;}
        public bool AllowedAllRoles{get;set;}
        public bool DeniedAll{get;set;}
    }
}