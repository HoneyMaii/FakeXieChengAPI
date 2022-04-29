using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FakeXieCheng.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FakeXieCheng.Database
{
  public class AppDbContext:DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext>options):base(options)
    {
      
    }

    public DbSet<TouristRoute> TouristRoutes { get; set; }
    public DbSet<TouristRoutePicture> TouristRoutePictures { get; set; }

    // 创建数据库表与模型映射关系时做补充说明用的
    // 可以创建自定义映射关系
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      // modelBuilder.Entity<TouristRoute>().HasData(new TouristRoute
      // {
      //   Id = Guid.NewGuid(),
      //   Title = "ceshititle",
      //   Description = "shuoming",
      //   CreateTime = DateTime.UtcNow
      // });
      // 使用 C# 的反射机制获取当前程序执行目录
      var touristRouteJsonData=File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+ @"/Database/touristRoutesMockData.json");
      IList<TouristRoute> touristRoutes = JsonConvert.DeserializeObject<IList<TouristRoute>>(touristRouteJsonData);
      modelBuilder.Entity<TouristRoute>().HasData(touristRoutes);

      var touristRoutePictureJsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/Database/touristRoutePicturesMockData.json");
      IList<TouristRoutePicture> touristPRoutePictures = JsonConvert.DeserializeObject<IList<TouristRoutePicture>>(touristRoutePictureJsonData);
      modelBuilder.Entity<TouristRoutePicture>().HasData(touristPRoutePictures);

      base.OnModelCreating(modelBuilder);
    }
  }
}
