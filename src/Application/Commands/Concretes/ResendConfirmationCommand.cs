using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using UsersService.src.Application.Commands.Interfaces;
using UsersService.Src.Application.Commands.Interfaces;

namespace UsersService.Src.Application.Commands.Concretes;

public class ResendConfirmationCodeCommand(
    AmazonCognitoIdentityProviderClient provider,
    IConfiguration configuration)
    : IResendConfirmationCodeCommand
{
    private readonly AmazonCognitoIdentityProviderClient _provider = provider;
    private readonly string _clientId = configuration["AWS:Cognito:ClientId"]!;

    public async Task<bool> ExecuteAsync(string email)
    {
        try
        {
            var request = new ResendConfirmationCodeRequest
            {
                Username = email,
                ClientId = _clientId,
            };

            var response = await _provider.ResendConfirmationCodeAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
        catch
        {
            return false;
        }
    }
}
