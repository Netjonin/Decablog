using DecaBlog.Data.Repositories.Interfaces;
using DecaBlog.Models;
using DecaBlog.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecaBlog.Data.Repositories.Implementations
{
    public class SquadRepository : ISquadRepository
    {
        private readonly DecaBlogDbContext _context;

        public SquadRepository(DecaBlogDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddSquad(Squad model)
        {
            await _context.AddAsync(model);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<List<Squad>> GetAllSquads()
        {
            return await _context.Squads.ToListAsync();
        }
        public async Task<Squad> GetSquad(string Id)
        {
            return await _context.Squads.Include(x => x.UserSquads).FirstOrDefaultAsync(x => x.Id == Id);
        }
    }
}