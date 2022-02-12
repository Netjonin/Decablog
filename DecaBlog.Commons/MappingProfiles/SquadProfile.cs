using AutoMapper;
using DecaBlog.Models;
using DecaBlog.Models.DTO;

namespace DecaBlog.MappingProfiles
{
    public class SquadProfile : Profile
    {
        public SquadProfile()
        {
            CreateMap<SquadToAddDto, Squad>();
            CreateMap<Squad, SquadMinInfoToReturnDto>();
            CreateMap<SquadMinInfoToReturnDto, Squad>();
        }
    }
}
