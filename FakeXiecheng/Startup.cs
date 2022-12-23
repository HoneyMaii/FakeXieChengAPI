using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;
using Autofac.Extras.DynamicProxy;
using FakeXieCheng.API.Database;
using FakeXieCheng.API.Models;
using FakeXieCheng.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FakeXieCheng.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>(); // 连接上下文关系对象

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var secretByte = Encoding.UTF8.GetBytes(Configuration["Authentication:SecretKey"]);
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true, // 验证token发布者
                        ValidIssuer = Configuration["Authentication:Issuer"],

                        ValidateAudience = true, // 验证 token 持有者
                        ValidAudience = Configuration["Authentication:Audience"],

                        ValidateLifetime = true, // 验证 token 是否过期
                        IssuerSigningKey = new SymmetricSecurityKey(secretByte) // token 私钥文件进入并加密
                    };
                });
            services.AddControllers(setupAction =>
                {
                    setupAction.ReturnHttpNotAcceptable = true; // 开启请求Header 请求类型处理
                    // setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                })
                .AddNewtonsoftJson(setupAction =>
                {
                    setupAction.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver();
                })
                .AddXmlDataContractSerializerFormatters() // 支持返回 xml 格式
                .ConfigureApiBehaviorOptions(setupAction =>
                {
                    setupAction.InvalidModelStateResponseFactory = context =>
                    {
                        var problemDetail = new ValidationProblemDetails(context.ModelState)
                        {
                            Type = "无所谓",
                            Title = "数据验证失败",
                            Status = StatusCodes.Status422UnprocessableEntity,
                            Detail = "请看详细说明",
                            Instance = context.HttpContext.Request.Path
                        };
                        problemDetail.Extensions.Add("traceId", context.HttpContext.TraceIdentifier); // 增加追踪 id
                        return new UnprocessableEntityObjectResult(problemDetail)
                        {
                            ContentTypes = {"application/problem+json"} // 配置响应的媒体类型，方便前段解析
                        };
                    };
                }) // 控制API controller 行为的服务：非法模型状态响应工厂
                ;
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo() {Title = "FakeXieChengAPI", Version = "v1"});

                    // 生成 xml 文件 drive the swagger docs
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    
                    c.IncludeXmlComments(xmlPath);
                    c.CustomOperationIds(description => description.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null); // 生成自定义 operationId
                })
                .AddSwaggerGenNewtonsoftSupport(); // swagger 添加 jsonPatch 支持
            // 每次发起请求时创建全新的数据仓库，请求结束时自动注销仓库
            // 不同请求之间的数据仓库完全独立，互不影响
            services.AddTransient<ITouristRouteRepository, TouristRouteRepository>();
            /*
             *  有且仅创建一个数据仓库，之后系统使用都会使用同一个示例。
             *  优点： 简单易用便于管理内存占用少效率高
             *  缺点：处理每个独立的请求时共用通道会造成数据污染
             */
            //services.AddSingleton
            /*
             *  介于 transient 和 singleton 之间，同时引入事务管理 transaction概念，
             * 将一系列的请求/操作整合起来，放在同一个事务中，事务有且仅创建一个数据仓库
             *  事务结束后系统自动注销仓库
             */
            //services.AddScoped
            
            
            services.AddDbContext<AppDbContext>(options =>
            {
                // options.UseSqlServer("server=localhost; Database=FakeXieChengDb; User Id=sa; Password=masterQu;");
                // options.UseSqlServer(Configuration["DbContext:ConnectionString"]);
                var connectionString = Configuration["DbContext:MySQLConnectionString"]!;
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            // 扫描 AutoMapper 的 profile 文件
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // UrlHelper 组件
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();

            // 添加自定义媒体类型格式处理器
            services.Configure<MvcOptions>(config =>
            {
                var outputFormatter = config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>().FirstOrDefault();
                if (outputFormatter != null)
                {
                    outputFormatter.SupportedMediaTypes.Add("application/vnd.eddy.hateoas+json");
                }
            });

            // 添加分布式缓存（用于保存幂等键的值和响应数据）
            services.AddDistributedMemoryCache();
        }

        // for AutoFac
        public void ConfigureContainer(ContainerBuilder builder)
        {
            // builder.RegisterType<MyService>().As<IMyService>();

            #region 命名注册

            // builder.RegisterType<MyServiceV2>().Named<IMyservice>("service2");

            #endregion

            #region 属性注册

            // builder.RegisterType<MyServiceV2>().As<IMyService>().PropertiesAutowired();

            #endregion

            #region AOP

            // builder.RegisterType<MyInterceptor>();
            // builder.RegisterType<MyNameService>();
            // builder.RegisterType<MyServiceV2>().As<IMyService>().PropertiesAutowired()
            //     .InterceptedBy(typeof(MyInterceptor));

            #endregion

            #region 子容器

            // builder.RegisterType<MyNameService>().InstancePerMatchingLifetimeScope("myscope"); // 把服务注入到特定名字的子容器中

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FakeXieCheng V1");
                    c.DisplayOperationId(); // 所有 operationId 显示在 swagger 中
                });
            }

            // 你在哪
            app.UseRouting();

            // 你是谁
            app.UseAuthentication();

            // 你可以干什么？有什么权限？
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}