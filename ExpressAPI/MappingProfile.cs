using AutoMapper;
using Entities.Models;
using Shared.DTO;

namespace ExpressAPI;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //User
        CreateMap<ApplicationUser, CreateUserDTO>().ReverseMap();
        CreateMap<ApplicationUser, GetUsersListDTO>().ReverseMap();
        CreateMap<ApplicationUser, LoginUserDTO>().ReverseMap();
        CreateMap<ApplicationUser, GetUserDetailsDTO>().ReverseMap();
        CreateMap<ApplicationUser, UpdateUserDTO>().ReverseMap();

    }
}