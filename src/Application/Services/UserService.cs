using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using AutoMapper;
using UsersService.Src.Application.DTOs;
using UsersService.Src.Application.Interfaces;
using UsersService.Src.Domain.Interfaces;

namespace UsersService.src.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly AmazonCognitoIdentityProviderClient _provider;
    private readonly CognitoUserPool _userPool;

    public UserService(IUserRepository userRepository, IMapper mapper, IConfiguration config)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _provider = new AmazonCognitoIdentityProviderClient(Amazon.RegionEndpoint.USEast2);
        _userPool = new CognitoUserPool(
            config["AWS:Cognito:UserPoolId"],
            config["AWS:Cognito:ClientId"],
            _provider);
    }

    public async Task<UserDTO?> GetByPublicIdAsync(Guid publicId)
    {
        var user = await _userRepository.GetByPublicIdAsync(publicId);
        return user == null ? null : _mapper.Map<UserDTO>(user);
    }

    public async Task<LoggedUserDTO?> LoginAsync(string email, string password)
    {
        var user = new CognitoUser(email, _userPool.ClientID, _userPool, _provider);

        var authRequest = new InitiateSrpAuthRequest { Password = password };

        try
        {
            var authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);
            Console.WriteLine(authResponse.AuthenticationResult);
            if (authResponse.AuthenticationResult == null)
            {
                return null;
            }

            var cognitoSub = user.UserID;
            var name = user.Username;

            var appUser = await _userRepository.GetByIdAsync(Guid.Parse(name));

            if (appUser != null)
            {
                var dto = _mapper.Map<LoggedUserDTO>(appUser);
                dto.AccessToken = authResponse.AuthenticationResult.AccessToken;
                dto.RefreshToken = authResponse.AuthenticationResult.RefreshToken;
                return dto;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}
