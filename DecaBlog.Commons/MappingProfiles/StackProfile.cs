using AutoMapper;
using DecaBlog.Models;
using DecaBlog.Models.DTO;

namespace DecaBlog.MappingProfiles
{
    public class StackProfile : Profile
    {
        public StackProfile()
        {
            CreateMap<StackToAddDto, Stack>();
            CreateMap<Stack, StackMinInfoToReturnDto>();
        }
    }
}
