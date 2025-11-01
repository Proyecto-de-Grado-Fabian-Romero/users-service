namespace UsersService.Src.Application.Commands.Concretes;

using System.IdentityModel.Tokens.Jwt;
using UsersService.Src.Application.Commands.Interfaces;

public class ValidateAccessTokenCommand : ICommand<string, bool>
{
    public Task<bool> ExecuteAsync(string accessToken)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);
            return Task.FromResult(token.ValidTo > DateTime.UtcNow);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}
