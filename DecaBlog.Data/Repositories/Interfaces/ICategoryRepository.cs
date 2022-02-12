using System.Threading.Tasks;

namespace DecaBlog.Data.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<bool> SaveChanges();
    }
}
