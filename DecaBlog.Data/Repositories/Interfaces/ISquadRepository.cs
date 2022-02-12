using DecaBlog.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DecaBlog.Data.Repositories.Interfaces
{
    public interface ISquadRepository
    {
        Task<bool> AddSquad(Squad model);
        Task<List<Squad>> GetAllSquads();
        Task<Squad> GetSquad(string Id);
    }
}
