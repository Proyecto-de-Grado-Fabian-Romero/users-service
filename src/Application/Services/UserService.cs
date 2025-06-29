using UsersService.src.Application.Commands.Interfaces;
using UsersService.Src.Application.Commands.Interfaces;
using UsersService.Src.Application.DTOs;
using UsersService.Src.Application.DTOs.Update;
using UsersService.Src.Application.Interfaces;

namespace UsersService.Src.Application.Services;

public class UserService(
    ICommand<(string, string), LoggedUserDTO?> loginUserCommand,
    ICommand<string, LoggedUserDTO?> getLoggedUserCommand,
    ICommand<Guid, UserDTO?> getUserByPublicIdCommand,
    ICommand<string, string?> refreshTokenCommand,
    ICommand<string?, bool> logoutUserCommand,
    ICommand<string, bool> validateAccessTokenCommand,
    ICommand<(Guid, UpdateUserRequestDTO), bool> updateUserCommand,
    ICommand<SignUpRequest, bool> signUpUserCommand,
    ICommand<ConfirmSignUpRequest, bool> confirmSignUpUserCommand,
    IResendConfirmationCodeCommand resendConfirmationCodeCommand,
    ICommand<(string, ChangePasswordRequest), bool> changePasswordCommand
) : IUserService
{
    private readonly ICommand<(string, string), LoggedUserDTO?> _loginUserCommand = loginUserCommand;
    private readonly ICommand<string, LoggedUserDTO?> _getLoggedUserCommand = getLoggedUserCommand;
    private readonly ICommand<Guid, UserDTO?> _getUserByPublicIdCommand = getUserByPublicIdCommand;
    private readonly ICommand<string, string?> _refreshTokenCommand = refreshTokenCommand;
    private readonly ICommand<string, bool> _validateAccessTokenCommand = validateAccessTokenCommand;
    private readonly ICommand<string?, bool> _logoutUserCommand = logoutUserCommand;
    private readonly ICommand<(Guid, UpdateUserRequestDTO), bool> _updateUserCommand = updateUserCommand;
    private readonly ICommand<SignUpRequest, bool> _signUpUserCommand = signUpUserCommand;
    private readonly ICommand<ConfirmSignUpRequest, bool> _confirmSignUpUserCommand = confirmSignUpUserCommand;
    private readonly IResendConfirmationCodeCommand _resendConfirmationCodeCommand = resendConfirmationCodeCommand;

    private readonly ICommand<(string, ChangePasswordRequest), bool> _changePasswordCommand = changePasswordCommand;

    public Task<bool> SignUpAsync(SignUpRequest request) =>
        _signUpUserCommand.ExecuteAsync(request);

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

    public Task<bool> LogoutAsync(string? refreshToken) =>
        _logoutUserCommand.ExecuteAsync(refreshToken);

    public Task<bool> UpdateUserAsync(Guid publicId, UpdateUserRequestDTO dto) =>
        _updateUserCommand.ExecuteAsync((publicId, dto));

    public Task<bool> ConfirmSignUpAsync(ConfirmSignUpRequest request) =>
        _confirmSignUpUserCommand.ExecuteAsync(request);

    public Task<bool> ResendConfirmationCodeAsync(string email) =>
        _resendConfirmationCodeCommand.ExecuteAsync(email);

    public Task<bool> ChangePasswordAsync(string accessToken, ChangePasswordRequest request) =>
        _changePasswordCommand.ExecuteAsync((accessToken, request));
}
