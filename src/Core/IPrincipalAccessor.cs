using System.Security.Claims;

namespace AuthorizationExtension.Core
{
    public interface IPrincipalAccessor
    {
        ClaimsPrincipal Principal { get; }
    }
}