using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace AuthorizationExtension.Core
{
    public class DenyAllAuthorizationRequirement:AuthorizationHandler<DenyAllAuthorizationRequirement>, IAuthorizationRequirement
    {
        public bool DeniedAll{get;set;}
        public DenyAllAuthorizationRequirement(bool deniedAll)
        {
            DeniedAll=deniedAll;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DenyAllAuthorizationRequirement requirement)
        {
            if(DeniedAll)
            {
                context.Fail();
            }
            return Task.CompletedTask;
        }
    }
}