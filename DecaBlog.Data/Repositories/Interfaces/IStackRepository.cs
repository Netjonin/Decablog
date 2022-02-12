using DecaBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecaBlog.Data.Repositories.Interfaces
{
    public interface IStackRepository
    {
        int AddStack(Stack stack);
        Stack GetStackByName(string name);
        Task<IEnumerable<Stack>> GetAllStacks();
        Task<Stack> GetStackById(string Id);
    }
}
