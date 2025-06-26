namespace UsersService.Src.Application.Commands.Concretes;

using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using AutoMapper;
using UsersService.Src.Application.Commands.Interfaces;
using UsersService.Src.Domain.Entities;
using UsersService.Src.Domain.Interfaces;

public class SignUpUserCommand(
    AmazonCognitoIdentityProviderClient provider,
    CognitoUserPool userPool,
    IUserRepository userRepository,
    IMapper mapper)
    : ICommand<src.SignUpRequest, bool>
{
    private readonly AmazonCognitoIdentityProviderClient _provider = provider;
    private readonly CognitoUserPool _userPool = userPool;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<bool> ExecuteAsync(src.SignUpRequest input)
    {
        try
        {
            var signUpRequest = new Amazon.CognitoIdentityProvider.Model.SignUpRequest
            {
                ClientId = _userPool.ClientID,
                Username = input.Email,
                Password = input.Password,
                UserAttributes =
                [
                    new () { Name = "email", Value = input.Email },
                    new () { Name = "name", Value = input.Name },
                    new () { Name = "phone_number", Value = input.Phone }
                ],
            };

            await _provider.SignUpAsync(signUpRequest);

            var user = _mapper.Map<User>(input);

            await _userRepository.AddAsync(user);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
