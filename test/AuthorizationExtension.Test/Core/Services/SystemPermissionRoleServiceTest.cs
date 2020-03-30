using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuthorizationExtension.Core;
using AuthorizationExtension.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using Xunit;

namespace AuthorizationExtension.Test.Core.Services
{
    public class SystemPermissionServiceTest
    {

        [Fact]
        public async Task UpdateAsyncTest()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddAuthorizationExtension();
            
            services.AddScoped<ISystemPermissionRoleStore< SystemPermissionRole>>(sp=>
                    new Mock<ISystemPermissionRoleStore< SystemPermissionRole>>().Object);

            services.AddScoped<ISystemPermissionUserStore< SystemPermissionUser>>(sp =>
                   new Mock<ISystemPermissionUserStore< SystemPermissionUser>>().Object);

            services.AddScoped<ISystemPermissionStore<SystemPermission>>(sp =>
            {
                var mockSystemPermissionStore = new Mock<ISystemPermissionStore<SystemPermission>>();
                mockSystemPermissionStore.Setup(s => s.UpdateAsync(It.IsAny<SystemPermission>(), It.IsAny<CancellationToken>()))
                                         .Returns((SystemPermission p,CancellationToken _)=>Task.FromResult(p));

                SystemPermission[] permissions = new SystemPermission[]
                {
                    new SystemPermission(){Id="permission0",Name="permission0",AllowedAllRoles=true},
                    new SystemPermission(){Id="permission1",Name="permission1",AllowedAnonymous=true},
                    new SystemPermission(){Id="permission2",Name="permission2",DeniedAll=true},
                    new SystemPermission(){Id="permission3",Name="permission3"},
                };
                mockSystemPermissionStore.Setup(s=>s.FindByIdAsync(It.IsIn("permission0","permission1","permission2","permission3","permission4"),It.IsAny<CancellationToken>()))
                                         .Returns((string id,CancellationToken _)=>Task.FromResult(permissions.FirstOrDefault(p=>p.Id==id)));
                return mockSystemPermissionStore.Object;
            });

           var mockPermissionMonitor = new Mock<IPermissionMonitor>();
            services.AddScoped<IPermissionMonitor>(sp =>
            {
                mockPermissionMonitor.Setup(p => p.OnPermissionChangedAsync(It.IsAny<string>()));
                return mockPermissionMonitor.Object;
            });
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            using IServiceScope serviceScope = serviceProvider.CreateScope();
            ISystemPermissionService< SystemPermission> systemPermissionService =
            serviceScope.ServiceProvider.GetRequiredService<ISystemPermissionService< SystemPermission>>();
            
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            await systemPermissionService.UpdateAsync(new SystemPermission(){Id="permission0",Name="permission00",AllowedAllRoles=true},cancellationTokenSource.Token);
            mockPermissionMonitor.Verify(p=>p.OnPermissionChangedAsync("permission0"),Times.Never(),"未改动相关属性，触发调用了OnPermissionChangedAsync(\"permission0\")");
             await systemPermissionService.UpdateAsync(new SystemPermission(){Id="permission0",Name="permission00",AllowedAllRoles=false},cancellationTokenSource.Token);
            mockPermissionMonitor.Verify(p=>p.OnPermissionChangedAsync("permission0"),Times.AtLeastOnce(),"改动相关属性AllowedAllRoles，未调用OnPermissionChangedAsync(\"permission0\")");
             await systemPermissionService.UpdateAsync(new SystemPermission(){Id="permission1",Name="permission1",AllowedAnonymous=false},cancellationTokenSource.Token);
            mockPermissionMonitor.Verify(p=>p.OnPermissionChangedAsync("permission1"),Times.AtLeastOnce(),"改动相关属性AllowedAnonymous，未调用OnPermissionChangedAsync(\"permission1\")");
            await systemPermissionService.UpdateAsync(new SystemPermission(){Id="permission2",Name="permission2",DeniedAll=false},cancellationTokenSource.Token);
            mockPermissionMonitor.Verify(p=>p.OnPermissionChangedAsync("permission2"),Times.AtLeastOnce(),"改动相关属性DeniedAll，未调用OnPermissionChangedAsync(\"permission2\")");
            await systemPermissionService.UpdateAsync(new SystemPermission(){Id="permission3",Name="permission3",AllowedAllRoles=true,AllowedAnonymous=true,DeniedAll=true},cancellationTokenSource.Token);
            mockPermissionMonitor.Verify(p=>p.OnPermissionChangedAsync("permission3"),Times.AtLeastOnce(),"改动相关属性，未调用OnPermissionChangedAsync(\"permission3\")");
             
             Task task= systemPermissionService.UpdateAsync(new SystemPermission(){Id="permission4",Name="permission3",AllowedAllRoles=true,AllowedAnonymous=true,DeniedAll=true},cancellationTokenSource.Token);
             await ShouldThrowAsyncExtensions.ShouldThrowAsync<Exception>(task);

            // cacheManager.Set()
            //await systemPermissionRoleService.AddToRoleAsync(permissionId,roleId,CancellationToken.None);


        }
    }
}