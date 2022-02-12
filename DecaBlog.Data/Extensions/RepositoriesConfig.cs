using DecaBlog.Data.Repositories.Implementations;
using DecaBlog.Data.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DecaBlog.Data.Extensions
{
    public static class RepositoriesConfig
    {

        public static IServiceCollection ConfigureRepositories(this IServiceCollection services) {
            services.AddScoped<ISquadRepository, SquadRepository>();
            services.AddScoped<IInviteeRepository, InviteeRepository>();
            services.AddScoped<ISquadRepository, SquadRepository>();
            services.AddScoped<IStackRepository, StackRepository>();
            services.AddScoped<IArticleTopicRepository, ArticleTopicRepository>();
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ISquadRepository, SquadRepository>();
            services.AddScoped<IStackRepository, StackRepository>();        
            return services;
        }
    }
}
