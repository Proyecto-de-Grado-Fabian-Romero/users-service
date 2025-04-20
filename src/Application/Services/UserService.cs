using System;
using AutoMapper;
using UsersService.Src.Application.DTOs;
using UsersService.Src.Application.Interfaces;
using UsersService.Src.Domain.Interfaces;

namespace UsersService.src.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserDTO?> GetByPublicIdAsync(Guid publicId)
    {
        var user = await _userRepository.GetByPublicIdAsync(publicId);
        return user == null ? null : _mapper.Map<UserDTO>(user);
    }
}
