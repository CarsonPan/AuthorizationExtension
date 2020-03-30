using System.Collections.Generic;
using AuthorizationExtension.Models;
using Microsoft.AspNetCore.Authorization;

namespace AuthorizationExtension.Core
{
    public class AuthorizeData 
    {
        //原始AuthorizeAttribute 预留
        public IEnumerable<string> AuthenticationSchemes { get ; set; }
        public IEnumerable<string> Policies { get ; set; }

        public string ResoureId{get;set;}
        public bool AllowedAnonymous{get;set;}
        public bool AllowedAllRoles{get;set;}
        public bool DeniedAll{get;set;}
        public IEnumerable<string> AllowedUsers{get;set;}
        public IEnumerable<string> AllowedRoles{get;set;}
        
    }
}