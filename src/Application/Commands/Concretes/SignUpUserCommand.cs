namespace UsersService.Src.Application.Commands.Concretes;

using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.CognitoIdentityProvider.Model;
using AutoMapper;
using UsersService.Src.Application.Commands.Interfaces;
using UsersService.Src.Domain.Interfaces;

public class SignUpUserCommand(
    AmazonCognitoIdentityProviderClient provider,
    CognitoUserPool userPool,
    IUserRepository userRepository,
    IMapper mapper)
    : ICommand<SignUpRequest, bool>
{
    private readonly AmazonCognitoIdentityProviderClient _provider = provider;
    private readonly CognitoUserPool _userPool = userPool;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<bool> ExecuteAsync(SignUpRequest input)
    {
        try
        {
            var signUpRequest = new SignUpRequest
            {
                ClientId = _userPool.ClientID,
                Username = input.Email,
                Password = input.Password,
                UserAttributes = new List<AttributeType>
                {
                    new () { Name = "email", Value = input.Email },
                    new () { Name = "name", Value = input.Name },
                },
            };

            await _provider.SignUpAsync(signUpRequest);

            var user = new AppUser
            {
                Id = Guid.Parse(input.UserId),
                Email = input.Email,
                Name = input.Name,
                VerifiedEmail = false,
            };

            await _userRepository.AddAsync(user);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
