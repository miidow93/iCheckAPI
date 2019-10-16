using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iCheckAPI.Models;
using iCheckAPI.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace iCheckAPI
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
            services.AddCors();
            services.AddCors(opt => opt.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials().Build()));
            services.Configure<Settings>(
                opt =>
                {
                    opt.ConnectionString = Configuration.GetSection("MongoDBConnectionSettings:ConnectionString").Value;
                    opt.DatabaseName = Configuration.GetSection("MongoDBConnectionSettings:DatabaseName").Value;
                });


            services.AddDbContext<ICheckContext>(ctx => ctx.UseSqlServer(Configuration.GetConnectionString("ICheck")).EnableSensitiveDataLogging());

            /*services.Configure<Settings>(
                    Configuration.GetSection(nameof(Settings)));*/

            services.AddSingleton<ISettings>(sp => sp.GetRequiredService<IOptions<Settings>>().Value);
            // services.AddTransient<ICheckListRepo, CheckListRepo>();
            // services.AddScoped<ICheckListRepo, CheckListRepo>();
            services.AddScoped<IConducteurRepo, ConducteurRepo>();
            services.AddScoped<IVehiculeRepo, VehiculeRepo>();
            // services.AddSingleton<IConducteurRepo, ConducteurRepo>();
            services.AddSingleton<CheckListRepo>();



            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddMvc().AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);


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
                app.UseHsts();
            }

            app.UseCors("AllowAll");
            app.UseHsts();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
