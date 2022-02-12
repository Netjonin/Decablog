using AutoMapper;
using DecaBlog.Commons.Parameters;
using DecaBlog.Data.Repositories.Interfaces;
using DecaBlog.Helpers;
using DecaBlog.Models;
using DecaBlog.Models.DTO;
using DecaBlog.Services.Extensions;
using DecaBlog.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;


namespace DecaBlog.Services.Implementations
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IArticleTopicRepository _articleTopicRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;

        public ArticleService(IArticleTopicRepository articleTopicRepository, IArticleRepository articleRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _articleRepository = articleRepository;
            _articleTopicRepository = articleTopicRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        public async Task<List<ArticleByKeywordDto>> GetArticleByKeyword(string keyword)
        {
            var listToReturn = new List<Article>();
            var articleToReturn = new List<ArticleByKeywordDto>();
            var articleList = await _articleRepository.GetArticlesByKeyword(keyword);

            if (articleList.Count == 0)
            {
                return null;
            }

            for (int i = 0; i < articleList.Count; i++)
            {
                var keywordSplit = articleList[i].Keywords.Split(",");
                var result = keywordSplit.Contains(keyword);
                if (result)
                    listToReturn.Add(articleList[i]);
            }
            articleToReturn = _mapper.Map<List<ArticleByKeywordDto>>(listToReturn);
            return articleToReturn;
        }
        public async Task<PaginatedListDto<ArticlesToReturnByContributorIdDto>> GetArticleByContributorId(string contributorId, int pageNumber, int perPage)
        {
            var articles = _articleRepository.GetArticleByContributorId(contributorId);
            var returnDto = new List<ArticlesToReturnByContributorIdDto>();
            foreach (var article in articles)
            {
                var publisher = await _userManager.FindByIdAsync(article.PublisherId);
                returnDto.Add(new ArticlesToReturnByContributorIdDto
                {
                    ContributionId = article.Id,
                    ArticleText = article.ArticleText,
                    TopicId = article.ArticleTopicId,
                    Keywords = article.Keywords,
                    SubTopic = article.SubTopic,
                    CreatedAt = article.DateCreated,
                    UpdatedAt = article.DateUpdated,
                    Topic = article.ArticleTopic.Topic,
                    Publisher = new PublisherDto
                    {
                        PublisherId = article.PublisherId,
                        PublisherBy = $"{publisher.FirstName} {publisher.LastName}"
                    }
                });
            }
            var result = returnDto.GroupBy(c => c.TopicId).SelectMany(x => x.Select(x => x)).Select(x => x).ToList();
            var paginatedList = PagedList<ArticlesToReturnByContributorIdDto>.Paginate(result, pageNumber, perPage);
            return paginatedList;
        }

        public async Task<PaginatedListDto<AllArticlesToReturnDto>> GetAllArticlesAsync(ArticleRequestParameter parameters)
        {
            var allArticlesReturns = _articleRepository.GetAllArticlesAsync();
            List<AllArticlesToReturnDto> tableMapping = new List<AllArticlesToReturnDto>();
            var returnDto = _mapper.Map<List<AllArticlesToReturnDto>>(allArticlesReturns);
            //iterate over the keywords to create tags for the article
            foreach (var item in returnDto)
            {
                var articles = allArticlesReturns
                    .Where(c => c.Id == item.TopicId)
                    .SelectMany(x => x.ArticleList)
                    .Where(x => x.Keywords != null);
                foreach (var art in articles)
                {
                    var sp = art.Keywords?.Split(',');
                    foreach (var sd in sp!)
                        item.Tags.Add(sd);
                }
                tableMapping.Add(item);
            }
            var paginatedList = PagedList<AllArticlesToReturnDto>.Paginate(tableMapping, parameters.PageNumber, parameters.PerPage);
            return paginatedList;
        }

        public async Task<PaginatedListDto<ArticlesToReturnByPublisherIdDto>> GetArticleByPublisherId(string publisherId, int pageNumber, int perPage)
        {
            var articles = _articleRepository.GetArticleByPublisherId(publisherId);
            if (articles is null) throw new Exception("Error occurred");
            var returnDto = new List<ArticlesToReturnByPublisherIdDto>();
            foreach (var article in articles)
            {
                returnDto.Add(new ArticlesToReturnByPublisherIdDto
                {
                    ContributionId = article.Id,
                    ArticleText = article.ArticleText,
                    TopicId = article.ArticleTopicId,
                    Keywords = article.Keywords,
                    SubTopic = article.SubTopic,
                    CreatedAt = article.DateCreated,
                    UpdatedAt = article.DateUpdated,
                    Topic = article.ArticleTopic.Topic,
                    Contributor = new ContributorDto
                    {
                        ContributorId = article.UserId,
                        ContributorName = $"{article.Contributor.FirstName} {article.Contributor.LastName}"
                    }
                });
            }
            var result = returnDto.GroupBy(c => c.TopicId).SelectMany(x => x.Select(x => x)).Select(x => x).ToList();
            return PagedList<ArticlesToReturnByPublisherIdDto>.Paginate(result, pageNumber, perPage);
        }

        private string GetUserId() =>
            _httpContextAccessor.HttpContext?.User.GetLoggedInUserId<string>();

        public async Task<CreatedContributionDTO> CreateContribution(AddContributionDTO model, string topicId, User user)
        {
            //create the article topic since you are the owner of the article
            var article = _mapper.Map<Article>(model);
            article.Contributor = user;
            article.ArticleTopicId = topicId;
            article.ArticleText = model.ArtlcleText;
            var createContribution = await _articleRepository.CreateContribution(article);
            if (!createContribution)
                return null;
            return _mapper.Map<CreatedContributionDTO>(article);
        }
         
        public async Task<PublisUnPublishArticleResponseDto> PublishArticleAsync(string articleId)
        {
            var currentUserAsPublisherId = GetUserId();
            var articlePublishedResponse = await _articleRepository.PublishArticleAsync(articleId, currentUserAsPublisherId);
            if (articlePublishedResponse is null) return null;
            await _articleRepository.SaveChangesAsync();
            return _mapper.Map<PublisUnPublishArticleResponseDto>(articlePublishedResponse);
        }

        public async Task<PublisUnPublishArticleResponseDto> UnPublishArticleAsync(string articleId)
        {
            var currentUserAsPublisherId = GetUserId();
            var articlePublishedResponse = await _articleRepository.UnPublishArticleAsync(articleId, currentUserAsPublisherId);
            if (articlePublishedResponse is null) return null;
            await _articleRepository.SaveChangesAsync();
            return _mapper.Map<PublisUnPublishArticleResponseDto>(articlePublishedResponse);
        }

        public async Task<bool> DeleteContribution(string contributionId, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var contribution = await _articleRepository.GetContribution(contributionId);
            if (contribution == null)
                return false;
            if (user != contribution.Contributor)
                return false;
            var delContribution = await _articleRepository.RemoveContribution(contribution);
            if (!delContribution)
                return false;
            return true;
        }

        public async Task<UpdatedContributionDTO> UpdateContribution(ContributionToUpdateDTO model, string articleId, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            //Check if Topic with the ID Exist
            var articleToEdit = await _articleRepository.GetContributionById(articleId);
            if (articleToEdit == null)
                return null;
            //Search with topic Id in the ArticleTopic Repository
            var articleTopicToEdit = await _articleTopicRepository.GetArticleById(articleToEdit.ArticleTopicId);
            //Assign the new values
            articleToEdit.SubTopic = model.SubTopic;
            articleToEdit.ArticleText = model.ArtlcleText;
            articleToEdit.Keywords = model.Keywords;
            articleToEdit.DateUpdated = DateTime.Now;
            articleToEdit.Contributor = user;
            //Article Topic Properties to be edited in Article Topic
            articleTopicToEdit.Abstract = model.Abstract;
            var contributionPropertyEditor = await _articleRepository.EditAsync(articleToEdit);
            //Update The Contribution
            var articlePropertyToEdit = await _articleTopicRepository.EditAsync(articleTopicToEdit);
            if (!contributionPropertyEditor && !articlePropertyToEdit)
                return null;
            var returnedMap = Mapper.Map<UpdatedContributionDTO>(articleToEdit);
            return _mapper.Map<UpdatedContributionDTO>(returnedMap);
        }

        public async Task<PaginatedListDto<PublishedContributionToReturnDto>> GetPublishedArticlesAsync(int pageNumber, int perPage)
        {
            var publishedArticles = _articleRepository.GetPublishedArticlesAsync()
                .Select(x => new PublishedContributionToReturnDto
                {
                    ArticleId = x.Id,
                    ContributorId = x.UserId,
                    TopicId = x.ArticleTopicId,
                    Topic = x.ArticleTopic.Topic,
                    Subtopic = x.SubTopic,
                    Keywords = x.Keywords,
                    ArticleText = x.ArticleText,
                    DateCreated = x.DateCreated,
                    DateUpdated = x.DateUpdated,
                    Publisher = _userManager.Users.Where(y => y.Id == x.PublisherId)
                    .Select(p => new PublisherToReturnDTO()
                    {
                        Id = p.Id,
                        Fullname = $"{p.FirstName} {p.LastName}"
                    }).FirstOrDefault()
                }).OrderBy(x => x.Topic);
            await Task.CompletedTask;
            return PagedList<PublishedContributionToReturnDto>.Paginate(publishedArticles, pageNumber, perPage);
        }

        public async Task<PaginatedListDto<PendingContributionToReturnDto>> GetPendingArticlesAsync(int pageNumber, int perPage)
        {
            var pendingArticles = _articleRepository.GetPendingArticlesAsync()
                .Select(x => new PendingContributionToReturnDto
                {
                    ArticleId = x.Id,
                    ContributorId = x.UserId,
                    TopicId = x.ArticleTopicId,
                    Topic = x.ArticleTopic.Topic,
                    Subtopic = x.SubTopic,
                    Keywords = x.Keywords,
                    ArticleText = x.ArticleText,
                    DateUpdated = x.DateUpdated,
                }).OrderBy(x => x.Topic);
            await Task.CompletedTask;
            return PagedList<PendingContributionToReturnDto>.Paginate(pendingArticles, pageNumber, perPage);
        }

        public async Task<PaginatedListDto<AllArticlesToReturnDto>> GetAllArticlesByCategoryIdAsync(string Id, ArticleRequestParameter parameters)
        {
            var allArticlesReturns = _articleRepository.GetAllArticlesByCategoryIdAsync(Id);
            var paginatedList = PagedList<AllArticlesToReturnDto>.Paginate(allArticlesReturns, parameters.PageNumber, parameters.PerPage);
            foreach (var art in paginatedList.Data)
            {
                var tags = _articleRepository.GetArticlesByTopicId(art.TopicId)
                  .Select(x => x.Keywords).ToList()
                  .Aggregate((s1, s2) => s1 + "," + s2).Split(',');
                art.Tags = new HashSet<string>(tags);
            }
            return paginatedList;
        }
    }
}