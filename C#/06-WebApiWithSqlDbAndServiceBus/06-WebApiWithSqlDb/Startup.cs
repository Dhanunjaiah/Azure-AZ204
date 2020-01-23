using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using _06_WebApiWithSqlDb.Data;
using _06_WebApiWithSqlDb.ServiceHelpers;
using Microsoft.Azure.ServiceBus;

namespace _06_WebApiWithSqlDb
{
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
            var connectionString = Configuration.GetConnectionString("DbConnection");
            services.AddDbContext<DemoDbContext>(options =>
            {
                if (string.IsNullOrEmpty(connectionString))
                    options.UseInMemoryDatabase("Demodb");
                else
                    options.UseSqlServer(connectionString);
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "Product API",
                    Description = "Products api"
                });
            });

            var sbConnectionString = Configuration.GetSection("ServiceBus")["ConnectionString"];
            if (!string.IsNullOrEmpty(sbConnectionString))
            {
                var sbHelper = new ServiceBusHelper(sbConnectionString, "orders");               
                sbHelper.RegisterEventHandler(ServiceBusHelper.ProcessMessagesAsync);
                services.AddSingleton<ServiceBusHelper>(sbHelper);
            }

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            var connectionString = Configuration.GetConnectionString("DbConnection");
            if (!string.IsNullOrEmpty(connectionString))
            {
                InitializeDatabase(app);
            }
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Products API");
                options.RoutePrefix = "";
            });
            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var database = serviceScope.ServiceProvider.GetService<DemoDbContext>().Database;
                database.Migrate();
            }
        }

        
    }
}
