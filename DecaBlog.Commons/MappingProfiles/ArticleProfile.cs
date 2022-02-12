using System.Linq;
using AutoMapper;
using DecaBlog.Models;
using DecaBlog.Models.DTO;

namespace DecaBlog.Commons.MappingProfiles
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<Article, ArticleInfoDto>().ReverseMap();
            CreateMap<Address, AddressDto>().ReverseMap();
            CreateMap<Article, ArticleByKeywordDto>()
                .ForMember("ArticleId", dest => dest.MapFrom(src => src.Id))
                .ForMember("KeyWord", src => src.MapFrom(src => src.Keywords));
            CreateMap<User, Contributor>()
                .ForMember("FullName", dest => dest.MapFrom(src => string.Join(' ', src.FirstName, src.LastName)));
            CreateMap<Article, PublisUnPublishArticleResponseDto>()
                .ForMember("Status", dest => dest.MapFrom(src => src.IsPublished ? "Published" : "UnPublished"))
                .ForMember("ArticleId", dest => dest.MapFrom(src => src.Id));
            CreateMap<AddContributionDTO, Article>();
            CreateMap<AddArticleDTO, ArticleTopic>();
            CreateMap<AddArticleTopicDTO, ArticleTopic>();
            CreateMap<CreatedContributionDTO, Article>();
            CreateMap<CreatedArticleDTO, ArticleTopic>();
            CreateMap<ContributionToUpdateDTO, ArticleTopic>();
            CreateMap<ContributionToUpdateDTO, Article>();
            CreateMap<Article, UpdatedContributionDTO>()
                .ForMember(dest => dest.Abstract, opt => opt.MapFrom(src => src.ArticleTopic.Abstract))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.ArticleTopic.CategoryId))
                .ForMember("FullName", dest => dest.MapFrom(src => string.Join(' ', src.Contributor.FirstName, src.Contributor.LastName)));
            CreateMap<UpdatedContributionDTO, ArticleTopic>();
            CreateMap<User, AuthorDto>()
                .ForMember("AuthorId", dest => dest.MapFrom(src => src.Id))
                .ForMember("FullName", dest => dest.MapFrom(src => string.Join(' ', src.FirstName, src.LastName)))
                .ForMember("AuthorPhotoUrl", dest => dest.MapFrom(src => src.Photos.FirstOrDefault()))
                .ForMember("Stack", dest => dest.MapFrom(src => src.UserStacks.Select(x => x.Stack.Name).FirstOrDefault()))
                .ForMember("Squad", dest => dest.MapFrom(src => src.UserSquads.Select(x => x.Squad.Name).FirstOrDefault()));
            CreateMap<ArticleTopic, AllArticlesToReturnDto>()
                .ForMember("TopicId", dest => dest.MapFrom(src => src.Id))
                .ForMember("Topic", dest => dest.MapFrom(src => src.Topic))
                .ForMember("Tags", o => o.Ignore())
                .ForMember("Coverphotourl", dest => dest.MapFrom(src => src.PhotoUrl));
        }
    }
}
