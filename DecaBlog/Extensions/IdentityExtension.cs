using DecaBlog.Data;
using DecaBlog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace DecaBlog.Extensions
{
    public static class IdentityExtension
    {
        public static void AddIdentityExtension(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<DecaBlogDbContext>()
                .AddDefaultTokenProviders();
        }
    }
}
