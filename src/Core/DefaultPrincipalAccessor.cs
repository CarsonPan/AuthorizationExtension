using System.Security.Claims;
using System.Threading;
namespace AuthorizationExtension.Core
{
    public class DefaultPrincipalAccessor : IPrincipalAccessor
    {
        public ClaimsPrincipal Principal => Thread.CurrentPrincipal as ClaimsPrincipal;
    }
}