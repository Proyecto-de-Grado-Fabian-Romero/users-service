using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using UsersService.Src.Application.Commands.Interfaces;
using UsersService.Src.Application.DTOs;

namespace UsersService.Src.Application.Commands.Concretes;

public class ResendConfirmationCodeCommand(
    AmazonCognitoIdentityProviderClient provider,
    IConfiguration configuration)
    : ICommand<ResendCodeRequest, bool>
{
    private readonly AmazonCognitoIdentityProviderClient _provider = provider;
    private readonly string _clientId = configuration["AWS:Cognito:ClientId"]!;

    public async Task<bool> ExecuteAsync(ResendCodeRequest req)
    {
        try
        {
            var request = new ResendConfirmationCodeRequest
            {
                Username = req.Email,
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
