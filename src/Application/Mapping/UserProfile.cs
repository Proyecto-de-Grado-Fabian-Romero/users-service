namespace UsersService.Src.Application.Mapping;

using AutoMapper;
using UsersService.src;
using UsersService.Src.Application.DTOs;
using UsersService.Src.Application.DTOs.BankPaymentData;
using UsersService.Src.Domain.Entities;
using UsersService.Src.Domain.Enums;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDTO>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

        CreateMap<User, LoggedUserDTO>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

        CreateMap<UserDTO, User>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Enum.Parse<UserRole>(src.Role)));

        CreateMap<LoggedUserDTO, User>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Enum.Parse<UserRole>(src.Role)));

        CreateMap<SignUpRequest, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.VerifiedEmail, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone));

        CreateMap<BankPaymentData, BankPaymentDataDTO>();
    }
}
