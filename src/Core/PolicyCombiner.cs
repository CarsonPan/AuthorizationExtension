using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace AuthorizationExtension.Core
{
    public class PolicyCombiner : IPolicyCombiner
    {
        protected readonly IAuthorizationPolicyProvider PolicyProvider;
        public PolicyCombiner(IAuthorizationPolicyProvider policyProvider)
        {
            PolicyProvider = policyProvider;
        }
        public async Task<AuthorizationPolicy> CombineAsync(AuthorizeData authorizeData)
        {

            // Avoid allocating enumerator if the data is known to be empty
            var skip = authorizeData == null;
            if (skip)
            { // If we have no policy by now, use the fallback policy if we have one
                var fallbackPolicy = await PolicyProvider.GetFallbackPolicyAsync();
                if (fallbackPolicy != null)
                {
                    return fallbackPolicy;
                }
            }
            else
            {
                AuthorizationPolicyBuilder policyBuilder = new AuthorizationPolicyBuilder();
                var useDefaultPolicy = true;
                if (authorizeData.DeniedAll)
                {
                    policyBuilder.AddRequirements(new DenyAllAuthorizationRequirement(true));
                    useDefaultPolicy = false;

                }
                else
                {
                    if (!authorizeData.AuthenticationSchemes.IsNullOrEmpty())
                    {
                        foreach (string scheme in authorizeData.AuthenticationSchemes)
                        {
                            policyBuilder.AuthenticationSchemes.Add(scheme.Trim());
                        }
                    }
                    // 假如允许所有角色访问，只需要求登录即可
                    if (authorizeData.AllowedAllRoles)
                    {
                        policyBuilder.RequireAuthenticatedUser();
                        useDefaultPolicy = false;
                    }
                    else
                    {
                        if (!authorizeData.Policies.IsNullOrEmpty())
                        {
                            foreach (string policyName in authorizeData.Policies)
                            {
                                var policy = await PolicyProvider.GetPolicyAsync(policyName);
                                if (policy == null)
                                {
                                    throw new InvalidOperationException($"找不到名为： '{policyName}'的策略！");
                                }
                                policyBuilder.Combine(policy);
                            }

                            useDefaultPolicy = false;
                        }
                        
                        if (!authorizeData.AllowedUsers.IsNullOrEmpty()||!authorizeData.AllowedRoles.IsNullOrEmpty())
                        {
                            policyBuilder.AddRequirements(new RolesOrUsersAuthorizationRequirement(authorizeData.AllowedUsers,authorizeData.AllowedRoles));
                            useDefaultPolicy = false;
                        }

                    }

                    if (useDefaultPolicy)
                    {
                        policyBuilder.Combine(await PolicyProvider.GetDefaultPolicyAsync());
                    }

                }
                return policyBuilder?.Build();

            }
            return null;
        }
    }
}