using UsersService.Src.Application.Commands.Interfaces;
using UsersService.Src.Application.DTOs;
using UsersService.Src.Application.Interfaces;

namespace UsersService.src.Application.Services;

public class UserService(
    ICommand<(string, string), LoggedUserDTO?> loginUserCommand,
    ICommand<string, LoggedUserDTO?> getLoggedUserCommand,
    ICommand<Guid, UserDTO?> getUserByPublicIdCommand,
    ICommand<string, string?> refreshTokenCommand,
    ICommand<string, bool> validateAccessTokenCommand) : IUserService
{
    private readonly ICommand<(string, string), LoggedUserDTO?> _loginUserCommand = loginUserCommand;
    private readonly ICommand<string, LoggedUserDTO?> _getLoggedUserCommand = getLoggedUserCommand;
    private readonly ICommand<Guid, UserDTO?> _getUserByPublicIdCommand = getUserByPublicIdCommand;
    private readonly ICommand<string, string?> _refreshTokenCommand = refreshTokenCommand;
    private readonly ICommand<string, bool> _validateAccessTokenCommand = validateAccessTokenCommand;

    public Task<UserDTO?> GetByPublicIdAsync(Guid publicId) =>
        _getUserByPublicIdCommand.ExecuteAsync(publicId);

    public Task<LoggedUserDTO?> LoginAsync(string email, string password) =>
        _loginUserCommand.ExecuteAsync((email, password));

    public Task<LoggedUserDTO?> GetUserFromAccessTokenAsync(string accessToken) =>
        _getLoggedUserCommand.ExecuteAsync(accessToken);

    public Task<string?> RefreshAccessTokenAsync(string refreshToken) =>
        _refreshTokenCommand.ExecuteAsync(refreshToken);

    public Task<bool> IsAccessTokenValidAsync(string accessToken) =>
        _validateAccessTokenCommand.ExecuteAsync(accessToken);
}
