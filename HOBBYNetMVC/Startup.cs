using BusinessLogic.Services;
using DataAccess.Context;
using Domain.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOBBYNetMVC
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
            services.AddDbContext<HobbyNetContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("DBConnection"));
               });

            services.AddScoped<UserService>();
            services.AddScoped<HobbyService>();
            services.AddScoped<ExplorePostsService>();
            services.AddScoped<LocationService>();
            services.AddScoped<AdvertService>();
            services.AddRazorPages();
            services.AddDefaultIdentity<User>()
            .AddEntityFrameworkStores<HobbyNetContext>();
            services.AddControllersWithViews();

            services.AddAuthentication()
             .AddGoogle(options =>
             {
                 options.ClientId = "309671515965-npac8tm527ok5amva1d95op6j1lim0bc.apps.googleusercontent.com";
                 options.ClientSecret = "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";
             });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Account/Login";
                options.AccessDeniedPath = $"/Account/AccessDenied";
               
            });

            services.AddLogging(loggingBuilder => {
                loggingBuilder.AddConsole()
                    .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information);
                loggingBuilder.AddDebug();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=User}/{action=Profile}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
