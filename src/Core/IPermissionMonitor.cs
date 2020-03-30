using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthorizationExtension.Core
{
    public interface IPermissionMonitor
    {
        Task OnPermissionChangedAsync(string permissionId);
        Task OnPermissionChangedAsync(IEnumerable<string> permissionIds);
        Task OnResourceChangedAsync(string resourceId);
    }
}