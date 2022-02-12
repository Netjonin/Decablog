using System;
using System.Linq;
using AutoMapper;
using DecaBlog.Models;
using DecaBlog.Models.DTO;

namespace DecaBlog.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserToReturnDto>();
            CreateMap<UserToRegisterDto, User>()
               .ForMember(dest => dest.UserName, opt => opt.MapFrom(u => u.Email));
            CreateMap<User, UserMinInfoToReturnDto>()
                .ForMember(dest => dest.Stack, opt => opt.MapFrom(u => u.UserStacks.Select(x => x.Stack == null ? "" : x.Stack.Name).FirstOrDefault()))
                .ForMember(dest => dest.Squad, opt => opt.MapFrom(u => u.UserSquads.Select(x => x.Squad == null ? "" : x.Squad.Name).FirstOrDefault()))
                .ForMember(dest => dest.Photo, opt => opt.MapFrom(u => u.PhotoUrl));
            CreateMap<AddressToUpdateDto, Address>().ForMember(x => x.UserId, y => y.Ignore());
            CreateMap<Address, AddressToReturnDto>().ForMember(dest => dest.Id, x => x.MapFrom(x => x.UserId));
            CreateMap<UserToUpdateDto, User>();
            CreateMap<User, UserInfoToReturnDto>();
            CreateMap<User, UserToReturnDto>();
            CreateMap<User, UserFullInfoDto>()
                .ForMember(x => x.Stack, opt => opt.MapFrom(s => s.UserStacks.Select(x => x.Stack == null ? "" : x.Stack.Name).FirstOrDefault()))
                .ForMember(x => x.Squard, opt => opt.MapFrom(s => s.UserSquads.Select(x => x.Squad == null ? "" : x.Squad.Name).FirstOrDefault()));
            CreateMap<Address, AddressDto>();
        }
    }
}
