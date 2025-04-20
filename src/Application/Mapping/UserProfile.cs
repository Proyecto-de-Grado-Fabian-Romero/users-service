namespace UsersService.Src.Application.Mapping;

using AutoMapper;
using UsersService.Src.Application.DTOs;
using UsersService.Src.Domain.Entities;
using UsersService.Src.Domain.Enums;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDTO>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.PhotoFileUrl, opt => opt.MapFrom(src =>
                !string.IsNullOrEmpty(src.PhotoFileId) ? $"/media/{src.PhotoFileId}" : null));

        CreateMap<User, LoggedUserDTO>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.PhotoFileUrl, opt => opt.MapFrom(src =>
                !string.IsNullOrEmpty(src.PhotoFileId) ? $"/media/{src.PhotoFileId}" : null));

        CreateMap<UserDTO, User>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Enum.Parse<UserRole>(src.Role)));

        CreateMap<LoggedUserDTO, User>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Enum.Parse<UserRole>(src.Role)));
    }
}
