using AutoMapper;
using DecaBlog.Data.Repositories.Interfaces;
using DecaBlog.Services.Interfaces;

namespace DecaBlog.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
    }
}
