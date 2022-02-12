using DecaBlog.Models;
using DecaBlog.Models.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DecaBlog.Data.Repositories.Interfaces
{
    public interface IArticleRepository
    {
        Task<List<Article>> GetArticlesByKeyword(string keyword);
        IQueryable<Article> GetPublishedArticlesAsync();
        IQueryable<Article> GetPendingArticlesAsync();
        Task<bool> CreateContribution(Article model);
        Task<List<Article>> GetArticlesAsync();
        IQueryable<Article> GetArticleByPublisherId(string publisherId);
        IQueryable<Article> GetArticleByContributorId(string contributorId);
        IQueryable<ArticleTopic> GetAllArticlesAsync();
        Task<Article> PublishArticleAsync(string articleId, string currentUserAsPublisherId);
        Task<Article> UnPublishArticleAsync(string articleId, string currentUserAsPublisherId);
        Task<bool> SaveChangesAsync();
        Task<Article> GetContribution(string contributionId);
        Task<bool> RemoveContribution(Article contribution);
        Task<bool> EditAsync(Article model);
        Task<Article> GetContributionById(string articleId);
        IQueryable<AllArticlesToReturnDto> GetAllArticlesByCategoryIdAsync(string categoryId);
        IQueryable<Article> GetArticlesByTopicId(string topicId);
    }
}