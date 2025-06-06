namespace UsersService.Src.Application.Commands.Concretes;

using UsersService.Src.Application.Commands.Interfaces;

public class LogoutUserCommand : ICommand<string?, bool>
{
    public Task<bool> ExecuteAsync(string? refreshToken)
    {
        return Task.FromResult(true);
    }
}
