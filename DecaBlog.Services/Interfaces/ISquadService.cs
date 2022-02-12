using DecaBlog.Models;
using DecaBlog.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DecaBlog.Services.Interfaces
{
    public interface ISquadService
    {
        Task<List<Squad>> GetAllSquads();
        Task<Squad> GetSquad(string Id);
        Task<SquadMinInfoToReturnDto> AddSquad(SquadToAddDto model);
    }
}
