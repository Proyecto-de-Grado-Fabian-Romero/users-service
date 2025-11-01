using System;
using System.Net;
using Amazon.CognitoIdentityProvider;
using UsersService.Src.Application.Commands.Interfaces;
using UsersService.Src.Application.DTOs;

namespace UsersService.src.Application.Commands.Concretes;

public class ConfirmForgotPasswordCommand(
    AmazonCognitoIdentityProviderClient provider,
    IConfiguration config
) : ICommand<ConfirmForgotPasswordRequest, bool>
{
    private readonly string _clientId = config["AWS:Cognito:ClientId"]!;

    public async Task<bool> ExecuteAsync(ConfirmForgotPasswordRequest input)
    {
        try
        {
            Console.WriteLine(input.NewPassword);
            var request = new Amazon.CognitoIdentityProvider.Model.ConfirmForgotPasswordRequest
            {
                ClientId = _clientId,
                Username = input.Email,
                ConfirmationCode = input.Code,
                Password = input.NewPassword,
            };

            var response = await provider.ConfirmForgotPasswordAsync(request);
            return response.HttpStatusCode == HttpStatusCode.OK;
        }
        catch
        {
            return false;
        }
    }
}
