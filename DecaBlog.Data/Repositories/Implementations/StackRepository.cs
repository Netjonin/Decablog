using DecaBlog.Data.Repositories.Interfaces;
using DecaBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DecaBlog.Data.Repositories.Implementations
{
    public class StackRepository : IStackRepository
    {
        private readonly DecaBlogDbContext _context;

        public StackRepository(DecaBlogDbContext context)
        {
            _context = context;
        }

        public int AddStack(Stack stack)
        {
            _context.Stacks.Add(stack);
            return _context.SaveChanges();
        }
        public Stack GetStackByName(string name)
        {
            return _context.Stacks.FirstOrDefault(x => x.Name == name);
        }
        public async Task<IEnumerable<Stack>> GetAllStacks()
        {
            return await _context.Stacks.ToListAsync();
        }
        public async Task<Stack> GetStackById(string Id)
        {
            return await _context.Stacks.FirstOrDefaultAsync(s => s.Id == Id);
        }
    }
}
