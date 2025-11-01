namespace UsersService.Src.Application.Commands.Concretes;

using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using UsersService.Src.Application.Commands.Interfaces;

public class RefreshAccessTokenCommand(AmazonCognitoIdentityProviderClient provider, CognitoUserPool userPool) : ICommand<string, string?>
{
    private readonly AmazonCognitoIdentityProviderClient _provider = provider;
    private readonly CognitoUserPool _userPool = userPool;

    public async Task<string?> ExecuteAsync(string refreshToken)
    {
        var request = new InitiateAuthRequest
        {
            AuthFlow = AuthFlowType.REFRESH_TOKEN_AUTH,
            ClientId = _userPool.ClientID,
            AuthParameters = new Dictionary<string, string>
            {
                { "REFRESH_TOKEN", refreshToken },
            },
        };

        var result = await _provider.InitiateAuthAsync(request);
        return result.AuthenticationResult?.AccessToken;
    }
}
