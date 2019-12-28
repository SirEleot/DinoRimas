using System;
using DinoRimas.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DinoRimas.Extensions
{
    
    public static class UserProviderExtension
    {
        public static void AddUser( this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<UserService>(); 
        }
        
    }
}
