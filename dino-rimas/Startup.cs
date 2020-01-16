using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using DinoRimas.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using DinoRimas.Extensions;
using DinoRimas.Models;

namespace DinoRimas
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
            services.AddControllersWithViews().AddJsonOptions(options=> {
                options.JsonSerializerOptions.MaxDepth = 620;
            });            

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddCookie(options =>
                {
                    options.LoginPath = "/User/SignIn";
                    options.LogoutPath = "/User/SignOut";
                })
                .AddSteam();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                });

            services.Configure<SettingsModel>(Configuration.GetSection("Settings"));
            services.AddDbContext<DinoRimasDbContext>(options =>options.UseNpgsql(Configuration.GetConnectionString("NpgSQL")));
            services.AddUser();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DinoRimasDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); 
                app.UseBrowserLink();
                ReloadDB(context, false);
            }
            else
            {
                //app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
                ReloadDB(context, false);
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
                    pattern: "{controller=DinoShop}/{action=Index}/{id?}");
            });
        }

        private void ReloadDB(DinoRimasDbContext context, bool deleteOld)
        {
            if(deleteOld) context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}
