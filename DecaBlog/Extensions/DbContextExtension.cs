using DecaBlog.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DecaBlog.Extensions
{
    public static class DbContextExtension
    {
        public static void AddDbContextExtension(this IServiceCollection services, IConfiguration config )
        {
            services.AddDbContextPool<DecaBlogDbContext>(options =>
            options.UseSqlite(config.GetConnectionString("Default")));
        }
    }
}
