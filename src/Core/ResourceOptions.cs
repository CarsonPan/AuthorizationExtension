using System.Collections.Generic;
namespace AuthorizationExtension.Core
{
    public class ResourceOptions
    {
        public IEnumerable<string> RequiredRouteKeys{get;set;}=new string[]{"action","area","controller","handler","page"};
        public IEnumerable<string> CustomRouteKeys{get;set;}
    }
}