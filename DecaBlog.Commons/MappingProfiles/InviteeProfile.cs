using AutoMapper;
using DecaBlog.Models;
using DecaBlog.Models.DTO;

namespace DecaBlog.MappingProfiles
{
    public class InviteeProfile : Profile
    {
        public InviteeProfile()
        {
            CreateMap<RegisterInvitedUserDto, Invitee>();
        }
    }
}
