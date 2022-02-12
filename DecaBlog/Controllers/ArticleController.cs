using DecaBlog.Commons.Helpers;
using DecaBlog.Models;
using System.Threading.Tasks;
using DecaBlog.Commons.Parameters;
using DecaBlog.Helpers;
using DecaBlog.Models.DTO;
using DecaBlog.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DecaBlog.Data.Repositories.Interfaces;

namespace DecaBlog.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly IArticleTopicService _articleTopicService;
        private readonly UserManager<User> _userManager;
      
        public ArticleController(IArticleService articleService, IArticleTopicService articleTopicService, UserManager<User> userManager)
        {
            _articleService = articleService;
            _articleTopicService = articleTopicService;
            _userManager = userManager;
        }

        [HttpGet("get-articles-by-categoryid")]
        public async Task<IActionResult> GetAllArticleByCategoryId([FromQuery] ArticleRequestParameter parameters, string categoryId)
        {
            var articlesResponse = await _articleService.GetAllArticlesByCategoryIdAsync(categoryId, parameters);

            if (articlesResponse != null)
                return Ok(ResponseHelper.BuildResponse<object>(true, "All articles successfully fetched for this category", ResponseHelper.NoErrors, articlesResponse));
            ModelState.AddModelError("", "Articles not found");
            return NotFound(ResponseHelper.BuildResponse<object>(false, "No records found", ModelState, null));
        }

        [HttpGet("get-article/{keyword}")]
        public async Task<IActionResult> GetArticleByKeyword(string keyword, [FromQuery] int pageNumber, [FromQuery]  int perPage)
        {

            if (string.IsNullOrWhiteSpace(keyword))
            {
                ModelState.AddModelError("", "Bad articleId format");
                return BadRequest(ResponseHelper.BuildResponse<object>(false, "Empty keyword", ModelState, null));
            }
            keyword = keyword.ToLower();
            var result = await _articleService.GetArticleByKeyword(keyword);

            if (result != null)
                return Ok(ResponseHelper.BuildResponse<object>(true, "Articles Was Successfully retrieved", ResponseHelper.NoErrors, result));

            return NotFound(ResponseHelper.BuildResponse<object>(false, "Article was not found", ResponseHelper.NoErrors, null));
        }

        [HttpGet("/api/v1/article/get-published-articles")]
        [Authorize(Roles = "Admin, Editor, Decadev")]
        public async Task<IActionResult> GetPublishedArticles([FromQuery] int pageNumber, [FromQuery] int perPage)
        {
            var publishedArticles = await _articleService.GetPublishedArticlesAsync(pageNumber, perPage);
            if (publishedArticles == null)
            {
                ModelState.AddModelError("Not found", "");
                return NotFound(ResponseHelper.BuildResponse<object>(false, "No any published articles found", ModelState, null));
            }
            return Ok(ResponseHelper.BuildResponse<object>(true, "Successfully fetched all published aricles", ResponseHelper.NoErrors, publishedArticles));
        }

        [HttpGet("/api/v1/article/get-pending-articles")]
        [Authorize(Roles = "Admin, Editor")]
        public async Task<IActionResult> GetPendingArticles([FromQuery] int pageNumber, [FromQuery] int perPage)
        {
            var pendingArticles = await _articleService.GetPendingArticlesAsync(pageNumber, perPage);
            if (pendingArticles == null)
            {
                ModelState.AddModelError("Not found", "");
                return NotFound(ResponseHelper.BuildResponse<object>(false, "No any pending articles found", ModelState, null));
            }
            return Ok(ResponseHelper.BuildResponse<object>(true, "Successfully fetched all pending aricles", ResponseHelper.NoErrors, pendingArticles));
        }

        [Authorize(Roles = "Admin, Editor, Decadev")]
        [HttpPost("add-article")]
        public async Task<IActionResult> CreateArticle([FromForm] AddArticleDTO model)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            var photo = model.Photo;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _articleTopicService.CreateNewArticle(model, photo, user);
            if (response == null)
            {
                ModelState.AddModelError("Failed To Add Article", "Article Addition Failed, Please Try again");
                return BadRequest(ResponseHelper.BuildResponse<string>(false, "Article failed to Add.", ModelState, null));
            }
            return Ok(ResponseHelper.BuildResponse<object>(true, "Article Added successful", ResponseHelper.NoErrors, response));
        }

        [Authorize(Roles = "Admin, Editor, Decadev")]
        [HttpPost("add-contribution")]
        public async Task<IActionResult> AddContribution([FromBody] AddContributionDTO model, string topicId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _articleService.CreateContribution(model, topicId, user);
            if (response == null)
            {
                ModelState.AddModelError("Failed To Add Article", "Article Addition Failed, Please Try again");
                return BadRequest(ResponseHelper.BuildResponse<string>(false, "Article failed to Add.", ModelState, null));
            }
            return Ok(ResponseHelper.BuildResponse<object>(true, "Contribution Added successful", ResponseHelper.NoErrors, response));
        }

        [HttpGet("get-articles")]
        public async Task<IActionResult> GetAllArticles([FromQuery] ArticleRequestParameter parameters)
        {
            var articlesResponse = await _articleService.GetAllArticlesAsync(parameters);
            if (articlesResponse != null)
                return Ok(ResponseHelper.BuildResponse<object>(true, "All articles successfully fetched", ResponseHelper.NoErrors, articlesResponse));
            ModelState.AddModelError("", "Article not found");
            return NotFound(ResponseHelper.BuildResponse<object>(false, "No records found", ModelState, null));
        }

        [HttpPatch("publish-article/{articleId}")]
        public async Task<IActionResult> PublishArticle([FromRoute] string articleId)
        {
            if (string.IsNullOrWhiteSpace(articleId))
            {
                ModelState.AddModelError("", "Bad articleId format");
                return BadRequest(ResponseHelper.BuildResponse<object>(false, "Empty articleId", ModelState, null));
            }
            var articleResponse = await _articleService.PublishArticleAsync(articleId);
            if (articleResponse != null)
                return Ok(ResponseHelper.BuildResponse<object>(true, "Article successfully published", ResponseHelper.NoErrors, articleResponse));
            ModelState.AddModelError("", "Article not found");
            return NotFound(ResponseHelper.BuildResponse<object>(false, "An error occured", ModelState, null));
        }

        [HttpPatch("unpublish-article/{articleId}")]
        public async Task<IActionResult> UnPublishArticle([FromRoute] string articleId)
        {
            if (string.IsNullOrWhiteSpace(articleId))
            {
                ModelState.AddModelError("", "Bad articleId format");
                return BadRequest(ResponseHelper.BuildResponse<object>(false, "Empty articleId", ModelState, null));
            }
            var articleResponse = await _articleService.UnPublishArticleAsync(articleId);
            if (articleResponse != null)
                return Ok(ResponseHelper.BuildResponse<object>(true, "Article successfully unpublished", ResponseHelper.NoErrors, articleResponse));
            ModelState.AddModelError("", "Article not found");
            return NotFound(ResponseHelper.BuildResponse<object>(false, "An error occured", ModelState, null));
        }

        [Authorize(Roles = "Admin, Editor, Decadev")]
        [HttpGet("get-article-by-id/{articleId}")]
        public async Task<IActionResult> GetArticleById(string articleId)
        {
            if (string.IsNullOrEmpty(articleId))
                return BadRequest(NotFound(ResponseHelper.BuildResponse<object>(false, "Article was not found", ModelState, null)));
            var article = await _articleTopicService.GetArticleById(articleId);
            if (article == null)
                return NotFound(ResponseHelper.BuildResponse<object>(false, "Article was not found", ModelState, null));
            return Ok(ResponseHelper.BuildResponse<object>(true, "Article found successfully", null, article));
        }

        [HttpGet("get-articles-by-contributorid/{contributorId}")]
        public async Task<IActionResult> GetArticleByContributorId(string contributorId, int pageNumber, int perPage) =>
           Ok(ResponseHelper.BuildResponse<object>(
               true, "Successfully fetched all contributions by contributionId", ModelState,
               await _articleService.GetArticleByContributorId(contributorId, pageNumber, perPage)));

        [HttpGet("get-articles-by-publisherid/{publisherId}")]
        public async Task<IActionResult> GetArticleByPublisherId(string publisherId, int pageNumber, int perPage) =>
            Ok(ResponseHelper.BuildResponse<object>(
                true, "Successfully fetched all contributions by publisherId", ModelState,
                await _articleService.GetArticleByPublisherId(publisherId, pageNumber, perPage)));

        [Authorize(Roles = "Editor, Decadev")]
        [HttpPut("update-contribution/{contributionId}")]
        public async Task<IActionResult> UpdateContribution([FromBody] ContributionToUpdateDTO model, [FromRoute] string contributionId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var response = await _articleService.UpdateContribution(model, contributionId, userId);
            if (response == null)
            {
                ModelState.AddModelError("Failed To Add Article", "Article Addition Failed, Please Try again");
                return BadRequest(ResponseHelper.BuildResponse<bool>(false, "Article failed to Add.", ModelState, false));
            }
            return Ok(ResponseHelper.BuildResponse<object>(true, "Article Updated successful", ResponseHelper.NoErrors, response));
        }

        [Authorize(Roles = "Admin,Editor, Decadev")]
        [HttpDelete("delete-contribution/{contributionId}")]
        public async Task<IActionResult> DeleteContribution(string contributionId)
        {
            if (string.IsNullOrEmpty(contributionId))
                ResponseHelper.BuildResponse<bool>(false, "No article Found", null, false);
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await _articleService.DeleteContribution(contributionId, userId);
            if (!response)
            {
                ModelState.AddModelError("Failed to Delete", "Failed to delete");
                return BadRequest(ResponseHelper.BuildResponse<string>(false, "Failed", ModelState, ""));
            }
            return Ok(ResponseHelper.BuildResponse<Article>(true, "Contribution was Successfully Deleted", null, null));
        }

        [HttpDelete("delete-article/{articleId}")]
        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> DeleteArticle(string articleId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseHelper.BuildResponse<object>(true, "Article was successful Deleted", ModelState, null));
            var response = await _articleTopicService.DeleteArticle(articleId);
            if (response == false)
            {
                ModelState.AddModelError("Failed to delete article", "Failed, Please Try again");
                return BadRequest(ResponseHelper.BuildResponse<string>(false, "Article failed to delete.", ModelState, null));
            }
            return Ok(ResponseHelper.BuildResponse<object>(true, "Article was successful Deleted", ResponseHelper.NoErrors, null));
        }
    }
}