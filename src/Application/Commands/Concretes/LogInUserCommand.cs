namespace UsersService.Src.Application.Commands.Concretes;

using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using UsersService.Src.Application.DTOs;
using UsersService.Src.Domain.Interfaces;
using AutoMapper;
using UsersService.Src.Application.Commands.Interfaces;

public class LoginUserCommand(AmazonCognitoIdentityProviderClient provider, CognitoUserPool userPool, IUserRepository repo, IMapper mapper) : ICommand<(string Email, string Password), LoggedUserDTO?>
{
    private readonly AmazonCognitoIdentityProviderClient _provider = provider;
    private readonly CognitoUserPool _userPool = userPool;
    private readonly IUserRepository _userRepository = repo;
    private readonly IMapper _mapper = mapper;

    public async Task<LoggedUserDTO?> ExecuteAsync((string Email, string Password) input)
    {
        var user = new CognitoUser(input.Email, _userPool.ClientID, _userPool, _provider);
        var authRequest = new InitiateSrpAuthRequest { Password = input.Password };

        var authResponse = await user.StartWithSrpAuthAsync(authRequest);
        if (authResponse.AuthenticationResult == null)
        {
            return null;
        }

        var appUser = await _userRepository.GetByIdAsync(Guid.Parse(user.Username));
        if (appUser == null)
        {
            return null;
        }

        var dto = _mapper.Map<LoggedUserDTO>(appUser);
        dto.AccessToken = authResponse.AuthenticationResult.AccessToken;
        dto.RefreshToken = authResponse.AuthenticationResult.RefreshToken;
        return dto;
    }
}