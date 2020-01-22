using System;
using DinoRimas.Models;
using DinoRimas.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DinoRimas.Extensions
{
    
    public static class DinoRimasProviderExtension
    {
        public static void AddUser( this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<UserService>(); 
        }

        public static void AddServerQuery(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<ServerQueryService>();
        }

    }
}
