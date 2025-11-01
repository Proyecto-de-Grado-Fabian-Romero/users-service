namespace UsersService.Src.Application.Commands.Concretes;

using UsersService.Src.Application.DTOs;
using UsersService.Src.Domain.Interfaces;
using AutoMapper;
using UsersService.Src.Application.Commands.Interfaces;

public class GetUserByPublicIdCommand(IUserRepository userRepository, IMapper mapper) : ICommand<Guid, UserDTO?>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<UserDTO?> ExecuteAsync(Guid publicId)
    {
        var user = await _userRepository.GetByPublicIdAsync(publicId);
        return user == null ? null : _mapper.Map<UserDTO>(user);
    }
}
