using DecaBlog.Models.DTO;
using System.Threading.Tasks;

namespace DecaBlog.Services.Interfaces
{
    public interface IMailService
    {
        Task SendMailAsync(EmailMessage contact);
    }
}
