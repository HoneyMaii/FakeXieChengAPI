using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FakeXieCheng.Database;
using FakeXieCheng.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Serialization;

namespace FakeXiecheng
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
      services.AddControllers(setupAction =>
      {
        setupAction.ReturnHttpNotAcceptable = true; // ��������Header �������ʹ���
        // setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
      })
        .AddNewtonsoftJson(setupAction =>
        {
          setupAction.SerializerSettings.ContractResolver =
            new CamelCasePropertyNamesContractResolver();
        })
        .AddXmlDataContractSerializerFormatters() // ֧�ַ��� xml ��ʽ
        .ConfigureApiBehaviorOptions(setupAction =>
        {
          setupAction.InvalidModelStateResponseFactory = context =>
          {
            var problemDetail = new ValidationProblemDetails(context.ModelState)
            {
              Type = "����ν",
              Title = "������֤ʧ��",
              Status = StatusCodes.Status422UnprocessableEntity,
              Detail = "�뿴��ϸ˵��",
              Instance = context.HttpContext.Request.Path
            };
            problemDetail.Extensions.Add("traceId", context.HttpContext.TraceIdentifier); // ����׷�� id
            return new UnprocessableEntityObjectResult(problemDetail)
            {
              ContentTypes = {"application/problem+json"} // ������Ӧ��ý�����ͣ�����ǰ�ν���
            };
          };
        
        }) // ����API controller ��Ϊ�ķ��񣺷Ƿ�ģ��״̬��Ӧ����
        ; 

      // ÿ�η�������ʱ����ȫ�µ����ݲֿ⣬�������ʱ�Զ�ע���ֿ�
      // ��ͬ����֮������ݲֿ���ȫ����������Ӱ��
      services.AddTransient<ITouristRouteRepository, TouristRouteRepository>();
      /*
       *  ���ҽ�����һ�����ݲֿ⣬֮��ϵͳʹ�ö���ʹ��ͬһ��ʾ����
       *  �ŵ㣺 �����ñ��ڹ����ڴ�ռ����Ч�ʸ�
       *  ȱ�㣺����ÿ������������ʱ����ͨ�������������Ⱦ
       */
      //services.AddSingleton
      /*
       *  ���� transient �� singleton ֮�䣬ͬʱ����������� transaction���
       * ��һϵ�е�����/������������������ͬһ�������У��������ҽ�����һ�����ݲֿ�
       *  ���������ϵͳ�Զ�ע���ֿ�
       */
      //services.AddScoped
      services.AddDbContext<AppDbContext>(options =>
      {
        // options.UseSqlServer("server=localhost; Database=FakeXieChengDb; User Id=sa; Password=masterQu;");
        // options.UseSqlServer(Configuration["DbContext:ConnectionString"]);
        var connectionString = Configuration["DbContext:MySQLConnectionString"];
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
      });

      // ɨ�� profile �ļ�
      services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
