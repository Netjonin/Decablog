using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DecaBlog.Data.Repositories.Interfaces;
using DecaBlog.Models;
using Microsoft.EntityFrameworkCore;

namespace DecaBlog.Data.Repositories.Implementations
{
    public class InviteeRepository : IInviteeRepository
    {
        private readonly DecaBlogDbContext _context;
        public InviteeRepository(DecaBlogDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddInvitee(Invitee invitee)
        {
            _context.Invitees.Add(invitee);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<Invitee> GetInviteeByEmail(string Email)
        {
            return await _context.Invitees.FirstOrDefaultAsync(x => x.Email.ToLower() == Email.ToLower());
        }
        public IOrderedQueryable<Invitee> GetInvitees()
        {
            var users = _context.Invitees.OrderBy(x => x.FirstName);
            return users;
        }
        public async Task<Invitee> GetInviteeById(string inviteId)
        {
            return await _context.Invitees.FirstOrDefaultAsync(x => x.InviteeId == inviteId);
        }

    }
}
