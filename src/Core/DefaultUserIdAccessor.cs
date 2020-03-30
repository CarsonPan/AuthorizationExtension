using System.Security.Claims;
using AuthorizationExtension.Core;

namespace AuthorizationExtension.Core
{
    public class DefaultUserIdAccessor : IUserIdAccessor
    {
        protected readonly IPrincipalAccessor PrincipalAccessor;
        public DefaultUserIdAccessor(IPrincipalAccessor principalAccessor)
        {
            PrincipalAccessor=principalAccessor;
        }
        public string UserId => PrincipalAccessor.Principal?.FindFirst(c=>c.Type==ClaimTypes.NameIdentifier)?.Value;
    }
}