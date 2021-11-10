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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<HobbyNetContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("DBConnection")));

            services.AddScoped<UserService>();
            services.AddScoped<HobbyService>();
            services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 5;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireDigit = false;               
            })
                .AddEntityFrameworkStores<HobbyNetContext>();
            // services.AddAuthorization(options =>
            //options.AddPolicy("admin",
            //    policy => policy.RequireClaim("Manager")));

            ////////////// auth

            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //     .AddCookie(options => //CookieAuthenticationOptions
            //    {
            //         options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
            //     });



            services.AddControllersWithViews();



           // services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest)
           //.AddRazorPagesOptions(options =>
           //{
           //    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
           //    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
           //});

            services.ConfigureApplicationCookie(options =>
            {
                //options.LoginPath = $"/Identity/Account/Login";
                //options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Account/AccessDenied";
               
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
