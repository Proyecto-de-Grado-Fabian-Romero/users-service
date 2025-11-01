namespace UsersService.Src.Application.Commands.Concretes;

using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using UsersService.Src.Application.DTOs;
using UsersService.Src.Domain.Interfaces;
using AutoMapper;
using UsersService.Src.Application.Commands.Interfaces;

public class GetLoggedUserCommand(AmazonCognitoIdentityProviderClient provider, IUserRepository repo, IMapper mapper) : ICommand<string, LoggedUserDTO?>
{
    private readonly AmazonCognitoIdentityProviderClient _provider = provider;
    private readonly IUserRepository _userRepository = repo;
    private readonly IMapper _mapper = mapper;

    public async Task<LoggedUserDTO?> ExecuteAsync(string accessToken)
    {
        var response = await _provider.GetUserAsync(new GetUserRequest { AccessToken = accessToken });
        var sub = response.UserAttributes.FirstOrDefault(a => a.Name == "sub")?.Value;

        if (string.IsNullOrEmpty(sub))
        {
            return null;
        }

        var user = await _userRepository.GetByIdAsync(Guid.Parse(sub));
        return user == null ? null : _mapper.Map<LoggedUserDTO>(user);
    }
}