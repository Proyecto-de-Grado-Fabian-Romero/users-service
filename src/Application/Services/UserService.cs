using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
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

    public async Task<LoggedUserDTO?> GetUserFromAccessTokenAsync(string accessToken)
    {
        try
        {
            var request = new Amazon.CognitoIdentityProvider.Model.GetUserRequest
            {
                AccessToken = accessToken,
            };

            var response = await _provider.GetUserAsync(request);
            var sub = response.UserAttributes.FirstOrDefault(a => a.Name == "sub")?.Value;
            Console.WriteLine(response);

            if (string.IsNullOrEmpty(sub))
            {
                return null;
            }

            var user = await _userRepository.GetByIdAsync(Guid.Parse(sub));
            return user == null ? null : _mapper.Map<LoggedUserDTO>(user);
        }
        catch
        {
            return null;
        }
    }

    public async Task<string?> RefreshAccessTokenAsync(string refreshToken)
    {
        var request = new InitiateAuthRequest
        {
            AuthFlow = AuthFlowType.REFRESH_TOKEN_AUTH,
            ClientId = _userPool.ClientID,
            AuthParameters = new Dictionary<string, string>
            {
                { "REFRESH_TOKEN", refreshToken },
            },
        };

        try
        {
            var result = await _provider.InitiateAuthAsync(request);
            return result.AuthenticationResult?.AccessToken;
        }
        catch
        {
            return null;
        }
    }

    public Task<bool> IsAccessTokenValidAsync(string accessToken)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);
            var exp = token.ValidTo;
            return Task.FromResult(exp > DateTime.UtcNow);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    private string GenerateSecretHash(string username, string clientId, string clientSecret)
    {
        var key = Encoding.UTF8.GetBytes(clientSecret);
        var message = Encoding.UTF8.GetBytes(username + clientId);
        using var hmac = new HMACSHA256(key);
        return Convert.ToBase64String(hmac.ComputeHash(message));
    }
}
