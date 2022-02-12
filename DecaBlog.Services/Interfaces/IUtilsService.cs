using DecaBlog.Models;
using DecaBlog.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecaBlog.Services.Interfaces
{
    public interface IUtilsService
    {
        Task<(IEnumerable<StackMinInfoToReturnDto>, IEnumerable<SquadMinInfoToReturnDto>)> GetAllSquadsAndStack();
    }
}
