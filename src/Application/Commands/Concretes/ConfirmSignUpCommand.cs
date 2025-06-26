using Amazon.CognitoIdentityProvider;
using UsersService.Src.Application.Commands.Interfaces;
using UsersService.Src.Application.DTOs;
using UsersService.Src.Domain.Interfaces;

namespace UsersService.src.Application.Commands.Concretes;

public class ConfirmSignUpCommand(
    AmazonCognitoIdentityProviderClient provider,
    IUserRepository userRepository,
    IConfiguration configuration)
    : ICommand<ConfirmSignUpRequest, bool>
{
    private readonly AmazonCognitoIdentityProviderClient _provider = provider;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly string _clientId = configuration["AWS:Cognito:ClientId"]!;

    public async Task<bool> ExecuteAsync(ConfirmSignUpRequest input)
    {
        try
        {
            var request = new Amazon.CognitoIdentityProvider.Model.ConfirmSignUpRequest
            {
                Username = input.Email,
                ConfirmationCode = input.Code,
                ClientId = _clientId,
            };

            var response = await _provider.ConfirmSignUpAsync(request);
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                var user = await _userRepository.GetByEmailAsync(input.Email);
                if (user != null)
                {
                    await _userRepository.MarkEmailAsVerifiedAsync(user.Id);
                }

                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}
