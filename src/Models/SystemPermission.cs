namespace AuthorizationExtension.Models
{
    public class SystemPermission
    {
        public string Id{get;set;}
        public string Name{get;set;}
        public string  Description{get;set;}
        public bool AllowAnonymous{get;set;}
        public bool AllowAllRoles{get;set;}
    }
}