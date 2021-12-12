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

            services.AddDefaultIdentity<User>(opt => {
                opt.Password.RequiredLength = 8;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireDigit = false; 
            })
            .AddEntityFrameworkStores<HobbyNetContext>();
            services.AddControllersWithViews();

            var googleClientData = Configuration.GetSection("GoogleClientData");
            var clientId = googleClientData.GetSection("ClientID").Value;
            var clientSecret = googleClientData.GetSection("ClientSecret").Value;

            services.AddAuthentication()
             .AddGoogle(options =>
             {
                 options.ClientId = clientId;
                 options.ClientSecret = clientSecret;
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
