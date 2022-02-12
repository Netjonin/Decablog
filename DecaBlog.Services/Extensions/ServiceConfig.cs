using System;
using System.Security.Claims;
using DecaBlog.Data.Extensions;
using DecaBlog.Services.Implementations;
using DecaBlog.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DecaBlog.Services.Extensions
{
    public static class ServiceConfig
    {
        public static IServiceCollection ConfigureCoreServices(this IServiceCollection services)
        {
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISquadService, SquadService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IStackService, StackService>();
            services.AddScoped<IStackService, StackService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<IArticleTopicService, ArticleTopicService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISquadService, SquadService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<IUtilsService, UtilsService>();
            services.AddScoped<IJwtService, JwtService>();
            services.ConfigureRepositories();
            return services;
        }

        public static T GetLoggedInUserId<T>(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            var loggedInUserId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (typeof(T) == typeof(string))
                return (T)Convert.ChangeType(loggedInUserId, typeof(T));
            if (typeof(T) == typeof(int) || typeof(T) == typeof(long))
            {
                return loggedInUserId != null
                    ? (T)Convert.ChangeType(loggedInUserId, typeof(T))
                    : (T)Convert.ChangeType(0, typeof(T));
            }
            throw new Exception("Invalid type provided");
        }
    }
}