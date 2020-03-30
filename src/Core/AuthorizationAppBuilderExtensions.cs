using System;
using AuthorizationExtension.Core;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.Builder
{
    public static class AuthorizationAppBuilderExtensions
    {
        public static IApplicationBuilder UseAuthorizationExtension(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            // VerifyServicesRegistered(app);

            return app.UseMiddleware<AuthorizationExtensionMiddleware>();
        }
        private static void VerifyServicesRegistered(IApplicationBuilder app)
        {
            // // Verify that AddAuthorizationPolicy was called before calling UseAuthorization
            // // We use the AuthorizationPolicyMarkerService to ensure all the services were added.
            // if (app.ApplicationServices.GetService(typeof(AuthorizationPolicyMarkerService)) == null)
            // {
            //     throw new InvalidOperationException(Resources.FormatException_UnableToFindServices(
            //         nameof(IServiceCollection),
            //         nameof(PolicyServiceCollectionExtensions.AddAuthorization),
            //         "ConfigureServices(...)"));
            // }
        }
    }
}