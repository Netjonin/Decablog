using AutoMapper;
using DecaBlog.Data.Repositories.Interfaces;
using DecaBlog.Models;
using DecaBlog.Models.DTO;
using DecaBlog.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace DecaBlog.Services.Implementations
{
    public class ArticleTopicService : IArticleTopicService
    {
        private readonly IArticleTopicRepository _articleTopicRepository;
        private readonly IMapper _mapper;
        private readonly IArticleService _articleService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly ICloudinaryService _cloudinaryService;

        public ArticleTopicService(ICloudinaryService cloudinaryService, IArticleTopicRepository articleTopicRepository, IArticleService articleService,
            IMapper mapper, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _articleTopicRepository = articleTopicRepository;
            _mapper = mapper;
            _articleService = articleService;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ArticleToReturnDto> GetArticleById(string id)
        {
            var result = await _articleTopicRepository.GetArticleById(id);
            if (result == null)
                return null;
            ArticleToReturnDto articleToReturn = new ArticleToReturnDto();
            articleToReturn.Articles = result.ArticleList.Select(x => new SubTopic { SubTopicName = x.SubTopic, Text = x.ArticleText, Date = x.DateCreated, SubId = x.Id, Publisher = null }).ToList();
            articleToReturn.Topic = new Topic
            {
                Abstract = result.Abstract,
                Category = result.Category.Name,
                PhotoUrl = result.PhotoUrl,
                TopicName = result.Topic,
                Date = result.DateCreated,
                TopicId = result.Id,
                PublicId = result.PublicId,
            };
            foreach (var article in result.ArticleList)
            {
                var contributor = await _userManager.FindByIdAsync(article.UserId);
                articleToReturn.Contributors.Add(new Contributor { Id = contributor.Id, FullName = $"{contributor.FirstName} {contributor.LastName}" });
                articleToReturn.Tags.Add(article.Keywords);
            }
            return articleToReturn;
        }

        public async Task<CreatedArticleDTO> CreateNewArticle(AddArticleDTO model, IFormFile photo, User user)
        {
            PhotoUploadResult uploadResult = new PhotoUploadResult();
            if (photo != null)
            {
                uploadResult = await _cloudinaryService.UploadPhoto(photo);
                if (uploadResult == null)
                    return null;
            }
            //create the article topic since you are the owner of the article
            var articleTopic = _mapper.Map<ArticleTopic>(model);
            articleTopic.Author = user;
            articleTopic.CategoryId = model.CategoryId;
            if (photo != null)
            {
                articleTopic.PublicId = uploadResult.PublicId;
                articleTopic.PhotoUrl = uploadResult.Url;
            }
            var createTheArticleTopic = await _articleTopicRepository.AddArticleTopic(articleTopic);
            if (createTheArticleTopic == false)
                return null;
            //create the article with the topic id
            var theArticle = new AddContributionDTO
            {
                SubTopic = model.SubTopic,
                ArtlcleText = model.ArtlcleText,
                Keywords = model.Keywords
            };
            var createContribution = await _articleService.CreateContribution(theArticle, articleTopic.Id, user);
            if (createContribution == null)
                return null;
            return _mapper.Map<CreatedArticleDTO>(articleTopic); ;
        }

        public async Task<bool> DeleteArticle(string articleId)
        {
            //Get the Article to be be delete
            var article = await _articleTopicRepository.GetArticleById(articleId);
            if (article == null)
                return false;
            //Perform the operation
            var articleToDelete = await _articleTopicRepository.DeleteArticle(article);
            return articleToDelete;
        }
    }
}