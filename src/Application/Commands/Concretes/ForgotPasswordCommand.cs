using System.Net;
using Amazon.CognitoIdentityProvider;
using UsersService.Src.Application.Commands.Interfaces;
using UsersService.Src.Application.DTOs;

namespace UsersService.Src.Application.Commands.Concretes;

public class ForgotPasswordCommand(
    AmazonCognitoIdentityProviderClient provider,
    IConfiguration config
) : ICommand<ForgotPasswordRequest, bool>
{
    private readonly string _clientId = config["AWS:Cognito:ClientId"]!;

    public async Task<bool> ExecuteAsync(ForgotPasswordRequest req)
    {
        try
        {
            var request = new Amazon.CognitoIdentityProvider.Model.ForgotPasswordRequest
            {
                ClientId = _clientId,
                Username = req.Email,
            };

            var response = await provider.ForgotPasswordAsync(request);
            return response.HttpStatusCode == HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }
}
