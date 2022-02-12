using DecaBlog.Data.Repositories.Interfaces;
using System.Threading.Tasks;

namespace DecaBlog.Data.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DecaBlogDbContext _context;
        public CategoryRepository(DecaBlogDbContext context)
        {
            _context = context;
        }
        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
