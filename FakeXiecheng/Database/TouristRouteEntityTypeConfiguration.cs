using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FakeXieCheng.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FakeXieCheng.API.Database;

public class TouristRouteEntityTypeConfiguration : IEntityTypeConfiguration<TouristRoute>
{
    public void Configure(EntityTypeBuilder<TouristRoute> builder)
    {
        // 使用 C# 的反射机制获取当前程序执行目录
        var touristRouteJsonData =
            File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                             @"/Database/touristRoutesMockData.json");
        IList<TouristRoute> touristRoutes =
            JsonConvert.DeserializeObject<IList<TouristRoute>>(touristRouteJsonData);
        if (touristRoutes != null) builder.HasData(touristRoutes);
    }
}