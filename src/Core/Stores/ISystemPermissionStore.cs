using System.Threading;
using System.Threading.Tasks;
using AuthorizationExtension.Models;

namespace AuthorizationExtension.Core
{
    public interface ISystemPermissionStore<TPermission> : IStoreBase<TPermission> where TPermission : SystemPermission
    {
    }
}