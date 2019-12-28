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

        // This method gets called by the runtime. Use this method to add services to the container.
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
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.Configure<SettingsModel>(Configuration.GetSection("Settings"));
            services.AddDbContext<DinoRimasDbContext>(options =>options.UseNpgsql(Configuration.GetConnectionString("NpgSQL")));
            services.AddUser();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DinoRimasDbContext context)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); 
                app.UseBrowserLink();
                ReloadDB(context, true);
            }
            else {            
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
