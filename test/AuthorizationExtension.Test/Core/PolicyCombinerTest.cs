using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthorizationExtension.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace AuthorizationExtension.Test
{
    public class PolicyCombinerTest
    {
        private IServiceProvider CreateServices()
        {
            var services = new ServiceCollection();

            services.AddAuthorizationExtension(options => options.AddPolicy("test", new AuthorizationPolicyBuilder().RequireUserName("admin").Build()));

            var serviceProvder = services.BuildServiceProvider();

            return serviceProvder;
        }
        [Fact]
        public async Task CombineAsyncTest()
        {
            IServiceProvider serviceProvider = CreateServices();
            using IServiceScope scope = serviceProvider.CreateScope();
            IPolicyCombiner policyCombiner = scope.ServiceProvider.GetRequiredService<IPolicyCombiner>();
            //如果使用AuthorizeAttribute
            AuthorizeData authorizeData = new AuthorizeData();
            var policy = await policyCombiner.CombineAsync(authorizeData);
            policy.AuthenticationSchemes.ShouldBeEmpty();
            policy.Requirements.Count().ShouldBe(1);
            policy.Requirements[0].ShouldBeOfType<DenyAnonymousAuthorizationRequirement>();


            authorizeData = new AuthorizeData() { Policies = new string[] { "test" } };
            policy = await policyCombiner.CombineAsync(authorizeData);
            policy.AuthenticationSchemes.ShouldBeEmpty();
            policy.Requirements.Count().ShouldBe(1);
            policy.Requirements[0].ShouldBeOfType<NameAuthorizationRequirement>();

            authorizeData = new AuthorizeData() { AllowedRoles = new string[] { "Administrator", "User" } };
            policy = await policyCombiner.CombineAsync(authorizeData);
            policy.AuthenticationSchemes.ShouldBeEmpty();
            policy.Requirements.Count().ShouldBe(1);
            policy.Requirements[0].ShouldBeOfType<RolesOrUsersAuthorizationRequirement>();

            authorizeData = new AuthorizeData() { AllowedRoles = new string[] { "Administrator", "User" }, Policies = new string[] { "test" } };
            policy = await policyCombiner.CombineAsync(authorizeData);
            policy.AuthenticationSchemes.ShouldBeEmpty();
            policy.Requirements.Count().ShouldBe(2);
            policy.Requirements.Count(r => r.GetType() == typeof(NameAuthorizationRequirement)).ShouldBe(1);
            policy.Requirements.Count(r => r.GetType() == typeof(RolesOrUsersAuthorizationRequirement)).ShouldBe(1);

            authorizeData = new AuthorizeData()
            {
                AllowedAllRoles = true
            };
            policy = await policyCombiner.CombineAsync(authorizeData);
            policy.AuthenticationSchemes.ShouldBeEmpty();
            policy.Requirements.Count().ShouldBe(1);
            policy.Requirements[0].ShouldBeOfType<DenyAnonymousAuthorizationRequirement>();

            authorizeData = new AuthorizeData()
            {
                DeniedAll = true
            };
            policy = await policyCombiner.CombineAsync(authorizeData);
            policy.AuthenticationSchemes.ShouldBeEmpty();
            policy.Requirements.Count().ShouldBe(1);
            policy.Requirements[0].ShouldBeOfType<DenyAllAuthorizationRequirement>();

            authorizeData = new AuthorizeData()
            {    
                AllowedRoles = new string[] { "Administrator", "User" },
                AllowedUsers = new string[] { "1", "2" }
            };
            policy = await policyCombiner.CombineAsync(authorizeData);
            policy.AuthenticationSchemes.ShouldBeEmpty();
            policy.Requirements.Count().ShouldBe(1);
            policy.Requirements[0].ShouldBeOfType<RolesOrUsersAuthorizationRequirement>();

            policy = await policyCombiner.CombineAsync(null);
            policy.ShouldBeNull();
        }
    }
}