using System.Collections.Generic;
using System.Security.Claims;

namespace AuthorizationExtension.Core
{
    public class DefaultRoleAccessor : IRoleAccessor
    {
        protected readonly IPrincipalAccessor PrincipalAccessor;
        public DefaultRoleAccessor(IPrincipalAccessor principalAccessor)
        {
            PrincipalAccessor=principalAccessor;
        }
        public IList<string> Roles => PrincipalAccessor.Principal?.FindFirst(c=>c.Type==ClaimTypes.Role)?.Value?.Split(",")??new string[0];
    }
}