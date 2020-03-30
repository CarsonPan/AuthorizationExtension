using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace AuthorizationExtension.Core
{
    public class AuthorizationExtensionMiddleware
    {
        /// <summary>
        /// <see cef="AuthorizationMiddleware">
        /// </summary>
        private const string AuthorizationMiddlewareInvokedWithEndpointKey = "__AuthorizationMiddlewareWithEndpointInvoked";
        private static readonly object AuthorizationMiddlewareWithEndpointInvokedValue = new object();

        private readonly RequestDelegate _next;
        private readonly IPolicyCombiner _policyCombiner;

        public AuthorizationExtensionMiddleware(RequestDelegate next, IPolicyCombiner policyCombiner)
        {
            _next = next??throw new ArgumentNullException(nameof(next));;
            _policyCombiner = policyCombiner??throw new ArgumentNullException(nameof(policyCombiner));
        }
       

        public async Task InvokeAsync(HttpContext context, IAuthorizeDataManager authorizeDataManager, IPolicyEvaluator policyEvaluator)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                // EndpointRoutingMiddleware uses this flag to check if the Authorization middleware processed auth metadata on the endpoint.
                // The Authorization middleware can only make this claim if it observes an actual endpoint.
                context.Items[AuthorizationMiddlewareInvokedWithEndpointKey] = AuthorizationMiddlewareWithEndpointInvokedValue;
            }
            var authorizeData = await authorizeDataManager.GetAuthorizeDataAsync();
            if (authorizeData?.AllowedAnonymous==true)
            {
                await _next(context);
                return;
            }

            var policy = await _policyCombiner.CombineAsync(authorizeData);

            if (policy == null)
            {
                await _next(context);
                return;
            }
            
            var authenticateResult = await policyEvaluator.AuthenticateAsync(policy, context);
            // Note that the resource will be null if there is no matched endpoint
            var authorizeResult = await policyEvaluator.AuthorizeAsync(policy, authenticateResult, context, resource: endpoint);
            if (authorizeResult.Challenged)
            {
                if (policy.AuthenticationSchemes.Any())
                {
                    foreach (var scheme in policy.AuthenticationSchemes)
                    {
                        await context.ChallengeAsync(scheme);
                    }
                }
                else
                {
                    await context.ChallengeAsync();
                }

                return;
            }
            else if (authorizeResult.Forbidden)
            {
                if (policy.AuthenticationSchemes.Any())
                {
                    foreach (var scheme in policy.AuthenticationSchemes)
                    {
                        await context.ForbidAsync(scheme);
                    }
                }
                else
                {
                    await context.ForbidAsync();
                }

                return;
            }

            await _next(context);
        }
    }
}