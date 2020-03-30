using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthorizationExtension.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using Xunit;

namespace AuthorizationExtension.Test.Core
{
    public class AuthorizationExtensionMiddlewareTest
    {

        [Fact]
        public async Task InvokeAsyncTest()
        {

            IServiceProvider serviceProvider = CreateServices();
            //未指定权限时且为指定FallbackPolicy时 匿名用户可访问
            using (IServiceScope serviceScope = serviceProvider.CreateScope())
            {
                IServiceProvider scopeServiceProvider = serviceScope.ServiceProvider;
                var next = new TestRequestDelegate();
                var middleware = CreateMiddleware(next.Invoke, scopeServiceProvider.GetRequiredService<IPolicyCombiner>());
                var context = GetHttpContext(scopeServiceProvider);
                IPolicyEvaluator policyEvaluator = scopeServiceProvider.GetRequiredService<IPolicyEvaluator>();
                await middleware.InvokeAsync(context, MockAuthorizeDataManager(null), policyEvaluator);
                next.Called.ShouldBeTrue();
            }

            //未指定权限但是指定FallbackPolicy时 
            serviceProvider = CreateServices(services =>
            {
                services.AddScoped<IAuthorizationPolicyProvider>(sp =>
                {
                    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                    var policyProvider = new Mock<IAuthorizationPolicyProvider>();
                    policyProvider.Setup(p => p.GetFallbackPolicyAsync()).ReturnsAsync(policy);
                    return policyProvider.Object;
                });
            });
            using (IServiceScope serviceScope = serviceProvider.CreateScope())
            {
                IServiceProvider scopeServiceProvider = serviceScope.ServiceProvider;
                var next = new TestRequestDelegate();
                var middleware = CreateMiddleware(next.Invoke, scopeServiceProvider.GetRequiredService<IPolicyCombiner>());
                var context = GetHttpContext(scopeServiceProvider);
                IPolicyEvaluator policyEvaluator = scopeServiceProvider.GetRequiredService<IPolicyEvaluator>();
                await middleware.InvokeAsync(context, MockAuthorizeDataManager(null), policyEvaluator);
                next.Called.ShouldBeFalse();
            }


            serviceProvider = CreateServices();
            using (IServiceScope serviceScope = serviceProvider.CreateScope())
            {
                IServiceProvider scopeServiceProvider = serviceScope.ServiceProvider;
                var next = new TestRequestDelegate();
                var middleware = CreateMiddleware(next.Invoke, scopeServiceProvider.GetRequiredService<IPolicyCombiner>());
                IPolicyEvaluator policyEvaluator = scopeServiceProvider.GetRequiredService<IPolicyEvaluator>();
                //允许匿名访问
                var context = GetHttpContext(scopeServiceProvider);
                await middleware.InvokeAsync(context, MockAuthorizeDataManager(new AuthorizeData() { AllowedAnonymous = true }), policyEvaluator);
                next.Called.ShouldBeTrue();

                //拒绝所有
                next.Reset();
                await middleware.InvokeAsync(context, MockAuthorizeDataManager(new AuthorizeData() { DeniedAll = true }), policyEvaluator);
                next.Called.ShouldBeFalse();
                //允许所有登录者 访问
                next.Reset();
                await middleware.InvokeAsync(context,MockAuthorizeDataManager(new AuthorizeData(){AllowedAllRoles=true}),policyEvaluator);
                next.Called.ShouldBeFalse();
                next.Reset();
                ClaimsPrincipal principal=new ClaimsPrincipal();
                ClaimsIdentity basicIdentity=new  ClaimsIdentity(new Claim[] {
                        new Claim("Permission", "CanViewPage"),
                        new Claim(ClaimTypes.Role, "Administrator"),
                        new Claim(ClaimTypes.Role, "User"),
                        new Claim(ClaimTypes.NameIdentifier, "John")},"Basic",ClaimTypes.NameIdentifier,ClaimTypes.Role);
                principal.AddIdentity(basicIdentity);
                context=GetHttpContext(scopeServiceProvider,principal);
                await middleware.InvokeAsync(context,MockAuthorizeDataManager(new AuthorizeData(){AllowedAllRoles=true}),policyEvaluator);
                next.Called.ShouldBeTrue();

                // 角色验证
                next.Reset();
                await middleware.InvokeAsync(context,MockAuthorizeDataManager(new AuthorizeData(){AllowedRoles=new string[]{"User"}}),policyEvaluator);
                next.Called.ShouldBeTrue();

                next.Reset();
                await middleware.InvokeAsync(context,MockAuthorizeDataManager(new AuthorizeData(){AllowedRoles=new string[]{"userRole"}}),policyEvaluator);
                next.Called.ShouldBeFalse();

                //用户Id
                next.Reset();
                await middleware.InvokeAsync(context,MockAuthorizeDataManager(new AuthorizeData(){AllowedUsers=new string[]{"user0"}}),policyEvaluator);
                next.Called.ShouldBeFalse();

                next.Reset();
                await middleware.InvokeAsync(context,MockAuthorizeDataManager(new AuthorizeData(){AllowedUsers=new string[]{"John"}}),policyEvaluator);
                next.Called.ShouldBeTrue();

                next.Reset();
                await middleware.InvokeAsync(context,MockAuthorizeDataManager(new AuthorizeData(){AllowedRoles= new string[]{"role0","role1"},AllowedUsers=new string[]{"user0"}}),policyEvaluator);
                next.Called.ShouldBeFalse();

                next.Reset();
                await middleware.InvokeAsync(context,MockAuthorizeDataManager(new AuthorizeData(){AllowedRoles= new string[]{"role0","role1"},AllowedUsers=new string[]{"John"}}),policyEvaluator);
                next.Called.ShouldBeTrue();

                next.Reset();
                await middleware.InvokeAsync(context,MockAuthorizeDataManager(new AuthorizeData(){AllowedRoles= new string[]{"Administrator","role1"},AllowedUsers=new string[]{"John00"}}),policyEvaluator);
                next.Called.ShouldBeTrue();

                 next.Reset();
                await middleware.InvokeAsync(context,MockAuthorizeDataManager(new AuthorizeData(){AllowedRoles= new string[]{"Administrator","role1"},AllowedUsers=new string[]{"John"}}),policyEvaluator);
                next.Called.ShouldBeTrue();

            }


        }


        private AuthorizationExtensionMiddleware CreateMiddleware(RequestDelegate requestDelegate = null, IPolicyCombiner policyCombiner = null)
        {
            requestDelegate = requestDelegate ?? ((context) => Task.CompletedTask);

            return new AuthorizationExtensionMiddleware(requestDelegate, policyCombiner);
        }

        private Endpoint CreateEndpoint(params object[] metadata)
        {
            return new Endpoint(context => Task.CompletedTask, new EndpointMetadataCollection(metadata), "Test endpoint");
        }
        private IServiceProvider CreateServices(Action<IServiceCollection> registerServices = null)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddLogging();
            services.AddAuthorizationExtension();
            services.AddSingleton(Mock.Of<IAuthenticationService>());
            registerServices?.Invoke(services);
            return services.BuildServiceProvider();
        }
        private HttpContext GetHttpContext(IServiceProvider serviceProvider, ClaimsPrincipal principal = null, Endpoint endpoint = null)
        {
            var httpContext = new DefaultHttpContext();
            if (endpoint != null)
            {
                httpContext.SetEndpoint(endpoint);
            }
            httpContext.RequestServices = serviceProvider;
            if (principal != null)
            {
                httpContext.User = principal;
            }

            return httpContext;
        }

        private IAuthorizeDataManager MockAuthorizeDataManager(AuthorizeData authorizeData)
        {
            var mockAuthorizeDataManager = new Mock<IAuthorizeDataManager>();
            mockAuthorizeDataManager.Setup(m => m.GetAuthorizeDataAsync()).Returns(Task.FromResult<AuthorizeData>(authorizeData));
            return mockAuthorizeDataManager.Object;
        }

      

        private class TestRequestDelegate
        {
            private readonly int _statusCode;

            public bool Called => CalledCount > 0;
            public int CalledCount { get; private set; }

            public TestRequestDelegate(int statusCode = 200)
            {
                _statusCode = statusCode;
            }

            public void Reset()
            {
                CalledCount = 0;
            }

            public Task Invoke(HttpContext context)
            {
                CalledCount++;
                context.Response.StatusCode = _statusCode;
                return Task.CompletedTask;
            }
        }
    }
}