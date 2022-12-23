using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FakeXieCheng.API.Models;
using FakeXieCheng.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FakeXieCheng.API.Database
{
    public class AppDbContext : IdentityDbContext<ApplicationUser> //DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<TouristRoute> TouristRoutes { get; set; }
        public DbSet<TouristRoutePicture> TouristRoutePictures { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<LineItem> LineItems { get; set; }
        public DbSet<Order> Orders { get; set; }

        // 创建数据库表与模型映射关系时做补充说明用的
        // 可以创建自定义映射关系
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*
             * 实体类型的所有配置提取到实现 IEntityTypeConfiguration<TEntity> 的单独类中。
             */
            #region [方式2--分组配置1]

            // new TouristRouteEntityTypeConfiguration().Configure(modelBuilder.Entity<TouristRoute>());
            // new TouristRoutePictureEntityTypeConfiguration().Configure(modelBuilder.Entity<TouristRoutePicture>());

            #endregion

            /*
             * 在给定程序集中应用实现 IEntityTypeConfiguration 的类型中指定的所有配置。
             */
            #region [方式2--分组配置2]
            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TouristRouteEntityTypeConfiguration).Assembly);

            #endregion
            
            // 初始化用户与角色的种子
            // 1 更新用户与角色的外键
            modelBuilder.Entity<ApplicationUser>(u =>
                u.HasMany(x => x.UserRoles).WithOne().HasForeignKey(ur => ur.UserId).IsRequired());
            // 2 添加管理员角色
            var adminRoleId = "308660dc-ae51-480f-824d-7dca6714c3e2";
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                });
            // 3 添加用户
            var adminUserId = "90184155-dee0-40c9-bb1e-b5ed07afc04e";
            var adminUser = new ApplicationUser
            {
                Id = adminUserId,
                UserName = "admin@fakexiecheng.com",
                NormalizedUserName = "admin@fakexiecheng.com".ToUpper(),
                Email = "admin@fakexiecheng.com",
                NormalizedEmail = "admin@fakexiecheng.com".ToUpper(),
                TwoFactorEnabled = false,
                EmailConfirmed = true,
                PhoneNumber = "123456",
                PhoneNumberConfirmed = false
            };
            var ph = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = ph.HashPassword(adminUser, "Fake123$");
            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);
            // 4 给用户加入管理员角色
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = adminUserId
                });
            base.OnModelCreating(modelBuilder);
        }
    }
}