using System.Threading;
using System.Threading.Tasks;
using AuthorizationExtension.Core;
using AuthorizationExtension.Data;
using AuthorizationExtension.Models;

namespace AuthorizationExtension.Stores
{
    public class SystemPermissionStore : StoreBase<SystemPermission, AuthorizationDbContext>, ISystemPermissionStore
    {
        public SystemPermissionStore(AuthorizationDbContext dbContext) : base(dbContext)
        {
        }
    }
}