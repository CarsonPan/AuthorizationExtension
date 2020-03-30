using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuthorizationExtension.Core;
using AuthorizationExtension.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using Xunit;

namespace AuthorizationExtension.Test.Core.Services
{
    public class SystemResourceServiceTest
    {
        [Fact]
        public async Task UpdateAsyncTest()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddAuthorizationExtension();
            services.AddScoped<ISystemResourceStore<SystemResource>>(sp =>
            {
                var mockResourceStore = new Mock<ISystemResourceStore<SystemResource>>();
                mockResourceStore.Setup(s=>s.UpdateAsync(It.IsAny<SystemResource>(),It.IsAny<CancellationToken>()))
                                 .Returns((SystemResource resource,CancellationToken _)=>Task.FromResult(resource));
                SystemResource[] resources=new SystemResource[]{
                    new SystemResource(){Id="/Account/Index",Name="Index",ResourceType=ResourceType.Api,PermissionId="permission0"},
                    new SystemResource(){Id="/Account/Create",Name="Create",ResourceType=ResourceType.Api,PermissionId="permission1"},
                };
                mockResourceStore.Setup(s=>s.FindByIdAsync(It.IsIn("/Account/Index","/Account/Create","null"),It.IsAny<CancellationToken>()))
                                 .Returns((string id,CancellationToken _)=>Task.FromResult(resources.FirstOrDefault(r=>r.Id==id)));
                return mockResourceStore.Object;
            });

            services.AddScoped<ISystemPermissionStore<SystemPermission>>(sp =>
                   new Mock<ISystemPermissionStore<SystemPermission>>().Object);

            var mockPermissionMonitor = new Mock<IPermissionMonitor>();
            services.AddScoped<IPermissionMonitor>(sp =>
            {
                mockPermissionMonitor.Setup(p => p.OnResourceChangedAsync(It.IsAny<string>()));
                return mockPermissionMonitor.Object;
            });
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            using IServiceScope serviceScope = serviceProvider.CreateScope();
            ISystemResourceService<SystemResource> systemResourceService =
            serviceScope.ServiceProvider.GetRequiredService<ISystemResourceService<SystemResource>>();

            await systemResourceService.UpdateAsync(new SystemResource(){Id="/Account/Index",Name="首页",ResourceType=ResourceType.Api,PermissionId="permission0"},CancellationToken.None);
            mockPermissionMonitor.Verify(p=>p.OnResourceChangedAsync("/Account/Index"),Times.Never(),"未改动相关属性，触发调用了OnResourceChangedAsync(\"/Account/Index\")");

            await systemResourceService.UpdateAsync(new SystemResource(){Id="/Account/Create",Name="首页",ResourceType=ResourceType.Api,PermissionId="permission3"},CancellationToken.None);
            mockPermissionMonitor.Verify(p=>p.OnResourceChangedAsync("/Account/Create"),Times.AtLeastOnce(),"改动相关属性，未触发调用了OnResourceChangedAsync(\"/Account/Create\")");
           
            Task task= systemResourceService.UpdateAsync(new SystemResource(){Id="null",Name="首页",ResourceType=ResourceType.Api,PermissionId="permission1"},CancellationToken.None);
            await ShouldThrowAsyncExtensions.ShouldThrowAsync<Exception>(task);


        }
    }
}