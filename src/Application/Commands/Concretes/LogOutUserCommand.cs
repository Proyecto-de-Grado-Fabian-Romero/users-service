namespace UsersService.Src.Application.Commands.Concretes;

using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using UsersService.Src.Application.Commands.Interfaces;

public class LogoutUserCommand(
    AmazonCognitoIdentityProviderClient provider,
    string clientId) : ICommand<string?, bool>
{
    private readonly AmazonCognitoIdentityProviderClient _provider = provider;
    private readonly string _clientId = clientId;

    public async Task<bool> ExecuteAsync(string? refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            return false;
        }

        try
        {
            var request = new RevokeTokenRequest
            {
                Token = refreshToken,
                ClientId = _clientId,
            };

            await _provider.RevokeTokenAsync(request);
            return true;
        }
        catch (Exception ex)
        {
            // Log the error
            Console.WriteLine($"Error revoking token: {ex.Message}");
            return false;
        }
    }
}