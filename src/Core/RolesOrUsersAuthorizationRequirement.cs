using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace AuthorizationExtension.Core
{
    public class RolesOrUsersAuthorizationRequirement : AuthorizationHandler<RolesOrUsersAuthorizationRequirement>, IAuthorizationRequirement
    {
        public IEnumerable<string> AllowedUsers { get; }
        public IEnumerable<string> AllowedRoles { get; }
        public RolesOrUsersAuthorizationRequirement(IEnumerable<string> allowedUsers, IEnumerable<string> allowedRoles)
        {

            if (!(allowedUsers?.Any() == true || allowedRoles?.Any() == true))
            {
                throw new InvalidOperationException("至少指定一个以上用户或者角色！");
            }
            AllowedUsers = allowedUsers;
            AllowedRoles=allowedRoles;
        }

        private bool IsInRole(AuthorizationHandlerContext context, RolesOrUsersAuthorizationRequirement requirement)
        {
            return requirement.AllowedRoles?.Any(r => context.User.IsInRole(r))==true;
        }
        private bool IsInUsers(AuthorizationHandlerContext context, RolesOrUsersAuthorizationRequirement requirement)
        {
            if(requirement.AllowedUsers.IsNullOrEmpty())
            {
                return false;
            }

            var principal = context.User;
            ClaimsIdentity identity = principal.Identity as ClaimsIdentity;
            string nameClaimType = identity.NameClaimType;
            string userId = identity.FindFirst(c => c.Type == identity.NameClaimType)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new Exception("不能正确获取UserId!");
            }
            return requirement.AllowedUsers.Any(u => u == userId);
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesOrUsersAuthorizationRequirement requirement)
        {
            if (context.User != null)
            {
                if (IsInRole(context,requirement)||IsInUsers(context,requirement))
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}