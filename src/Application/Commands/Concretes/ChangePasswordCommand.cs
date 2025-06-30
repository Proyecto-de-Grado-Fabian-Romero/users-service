using Amazon.CognitoIdentityProvider;
using UsersService.Src.Application.Commands.Interfaces;

namespace UsersService.Src.Application.Commands.Concretes;

public class ChangePasswordCommand(
    AmazonCognitoIdentityProviderClient provider
) : ICommand<(string accessToken, DTOs.ChangePasswordRequest request), bool>
{
    private readonly AmazonCognitoIdentityProviderClient _provider = provider;

    public async Task<bool> ExecuteAsync((string accessToken, DTOs.ChangePasswordRequest request) input)
    {
        var (accessToken, request) = input;

        try
        {
            var changeRequest = new Amazon.CognitoIdentityProvider.Model.ChangePasswordRequest
            {
                AccessToken = accessToken,
                PreviousPassword = request.CurrentPassword,
                ProposedPassword = request.NewPassword,
            };

            var response = await _provider.ChangePasswordAsync(changeRequest);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
        catch
        {
            return false;
        }
    }
}
