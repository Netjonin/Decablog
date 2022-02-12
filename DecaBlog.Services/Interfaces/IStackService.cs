using DecaBlog.Models;
using DecaBlog.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DecaBlog.Services.Interfaces
{
    public interface IStackService
    {
        StackMinInfoToReturnDto AddStack(StackToAddDto model);
        Stack GetStackByName(string name);
        Task<IEnumerable<StackMinInfoToReturnDto>> GetAllStacks();
        Task<StackMinInfoToReturnDto> GetStackById(string Id);
    }
}
