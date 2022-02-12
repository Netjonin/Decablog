using System.Threading.Tasks;
using DecaBlog.Models;
using System.Linq;

namespace DecaBlog.Data.Repositories.Interfaces
{
    public interface IInviteeRepository
    {
        Task<bool> AddInvitee(Invitee invitee);
        Task<Invitee> GetInviteeByEmail(string Email);
        IOrderedQueryable<Invitee> GetInvitees();
        Task<Invitee> GetInviteeById(string inviteId);

    }
}
