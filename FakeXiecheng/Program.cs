using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Autofac.Extensions.DependencyInjection;
using FakeXieCheng.API;
using FakeXieCheng.API.ConfigurationCustom;
using Microsoft.Extensions.Primitives;

namespace FakeXiecheng
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    // config.Add(new MyConfigurationSource());
                    config.AddMyConfiguration(); // 自定义配置数据源：实现定制化配置方案
                    var configurationRoot = config.Build();
                    ChangeToken.OnChange(() => configurationRoot.GetReloadToken(), () =>
                    {
                        Console.WriteLine($"LastTime:{configurationRoot["lastTime"]}");
                    });
                    Console.WriteLine($"开始了");
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
    
}