

using AuthorizationExtension.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationExtension.Data
{
    public class AuthorizationDbContext : DbContext
    {
        public AuthorizationDbContext(DbContextOptions<AuthorizationDbContext> options)
        : base(options)
        {

        }

        public virtual DbSet<SystemPermission> SystemPermissions{get;set;}
        public virtual DbSet<SystemResource> SystemResources{get;set;}
        public virtual DbSet<SystemPermissionRole> SystemPermissionRoles{get;set;}

        public virtual DbSet<SystemPermissionUser> SystemPermissionUsers{get;set;}
    }
}