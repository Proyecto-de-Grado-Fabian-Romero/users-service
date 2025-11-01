namespace UsersService.Src.Application.Commands.Concretes;

using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using AutoMapper;
using UsersService.Src.Application.Commands.Interfaces;
using UsersService.Src.Domain.Entities;
using UsersService.Src.Domain.Interfaces;

public class SignUpUserCommand(
    AmazonCognitoIdentityProviderClient provider,
    CognitoUserPool userPool,
    IUserRepository userRepository,
    IMapper mapper
) : ICommand<DTOs.SignUpRequest, bool>
{
    private readonly AmazonCognitoIdentityProviderClient _provider = provider;
    private readonly CognitoUserPool _userPool = userPool;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<bool> ExecuteAsync(DTOs.SignUpRequest input)
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
                    new() { Name = "email", Value = input.Email },
                    new() { Name = "name", Value = input.Name },
                    new() { Name = "name", Value = input.Name },
                    new() { Name = "phone_number", Value = "+591" + input.Phone },
                    new() { Name = "picture", Value = "none" },
                ],
            };

            await _provider.SignUpAsync(signUpRequest);

            var userResult = await _provider.AdminGetUserAsync(
                new AdminGetUserRequest { Username = input.Email, UserPoolId = _userPool.PoolID }
            );

            var sub = userResult.UserAttributes.FirstOrDefault(attr => attr.Name == "sub")?.Value;

            var user = _mapper.Map<User>(input);
            user.Id = Guid.Parse(sub ?? string.Empty);

            await _userRepository.AddAsync(user);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }
}
