using AuthorizationExtension.Core;
using AuthorizationExtension.Data;
using AuthorizationExtension.Models;

namespace AuthorizationExtension.Stores
{
    public class SystemResourceStore : StoreBase<SystemResource, AuthorizationDbContext>, ISystemResourceStore
    {
        public SystemResourceStore(AuthorizationDbContext dbContext) : base(dbContext)
        {
        }
    }
}