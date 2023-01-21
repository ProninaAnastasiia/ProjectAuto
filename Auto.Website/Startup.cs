using System;
using System.IO;
using System.Reflection;
using Auto.Data;
using Auto.Website.GraphQL.Schemas;
using Auto.Website.Hubs;
using EasyNetQ;
using GraphQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Auto.Website;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRouting(options => options.LowercaseUrls = true);
        services.AddControllersWithViews().AddNewtonsoftJson();
        services.AddSingleton<IAutoDatabase, AutoCsvFileDatabase>();
        
        services.AddSignalR();

        services.AddSwaggerGen(
            config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Auto API"
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                config.IncludeXmlComments(xmlPath);
            });

        services.AddGraphQL(builder => builder
                .AddNewtonsoftJson() // Сторонний генератор json
                .AddAutoSchema<AutoSchema>() // Добавляет middleware для обработки схемы
                .AddSchema<AutoSchema>() // Добавляет схему
            //.AddGraphTypes(typeof(VehicleGraphType).Assembly)
        );

        var bus = RabbitHutch.CreateBus(Configuration.GetConnectionString("AutoRabbitMQ"));
        services.AddSingleton(bus);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();
        else // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        app.UseHttpsRedirection();
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseGraphQL<AutoSchema>();
        app.UseGraphQLGraphiQL("/graphiql");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<AutoHub>("/hub");
            endpoints.MapControllerRoute(
                "default",
                "{controller=Home}/{action=Index}/{id?}");
        });
    }
}