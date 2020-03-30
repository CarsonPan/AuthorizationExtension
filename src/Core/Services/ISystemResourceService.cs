using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AuthorizationExtension.Models;

namespace AuthorizationExtension.Core
{
    public interface ISystemResourceService<TResource>:IServiceBase<TResource> 
        where TResource:SystemResource 
    {
        //Task<TResource> SetPermissionAsync(string resourceId,TPermission permission, CancellationToken cancellationToken);
        
        
    }
}