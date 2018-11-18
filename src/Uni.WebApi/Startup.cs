﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Uni.DataAccess.Data;

namespace Uni.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddFluentValidation(configuration =>
                {
                    // Add validators
                    AssemblyScanner.FindValidatorsInAssemblyContaining<Startup>().ForEach(pair =>
                    {
                        // RegisterValidatorsFromAssemblyContaining does this:
                        services.AddTransient(pair.InterfaceType, pair.ValidatorType);
                        // Also register it as its concrete type as well as the interface type
                        services.AddTransient(pair.ValidatorType);
                    });
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            services.AddEntityFrameworkSqlServer();
            services.AddDbContext<UniDbContext>(x =>
            {
                x.UseSqlServer(
                    Configuration.GetConnectionString("UniDbConnection"),
                    sql => sql.MigrationsAssembly(typeof(UniDbContext).Assembly.FullName)
                    );
//#if DEBUG
//                x.EnableSensitiveDataLogging();
//#endif
            });

            // Configure versions 
            services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });

            // Configure swagger
            services.AddSwaggerGen(options =>
            {
                // Specify api versions
                options.SwaggerDoc("v1",
                    new Info
                    {
                        Title = "Uni API",
                        Version = "v1"
                    });

                options.DescribeAllEnumsAsStrings();
                options.DescribeStringEnumsInCamelCase();
                options.AddFluentValidationRules();

                var xmlPath = Path.Combine(AppContext.BaseDirectory, "Uni.WebApi.xml");
                options.IncludeXmlComments(xmlPath);

                options.OperationFilter<RemoveVersionFromParameter>();

                options.DocumentFilter<ReplaceVersionWithExactValueInPath>();

                options.DocInclusionPredicate((version, desc) =>
                {
                    if (!desc.TryGetMethodInfo(out var methodInfo))
                    {
                        return false;
                    }

                    var versions = methodInfo.DeclaringType
                        .GetCustomAttributes<ApiVersionAttribute>(true)
                        .SelectMany(attr => attr.Versions);

                    return versions.Any(x => $"v{x}" == version);
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseScopedSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Uni API v1"); });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
