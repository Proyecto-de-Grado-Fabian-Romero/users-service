using UsersService.Src.Application.DTOs;
using UsersService.Src.Application.DTOs.Update;

namespace UsersService.Src.Application.Interfaces;

public interface IUserService
{
    Task<UserDTO?> GetByPublicIdAsync(Guid publicId);

    Task<LoggedUserDTO?> LoginAsync(LoginRequest loginRequest);

    Task<LoggedUserDTO?> GetUserFromAccessTokenAsync(string accessToken);

    Task<string?> RefreshAccessTokenAsync(string refreshToken);

    Task<bool> IsAccessTokenValidAsync(string accessToken);

    Task<bool> LogoutAsync(string? refreshToken);

    Task<bool> UpdateUserAsync(Guid publicId, UpdateUserRequestDTO dto);

    Task<bool> SignUpAsync(SignUpRequest request);

    Task<bool> ConfirmSignUpAsync(ConfirmSignUpRequest request);

    Task<bool> ResendConfirmationCodeAsync(ResendCodeRequest email);

    Task<bool> ChangePasswordAsync(string accessToken, ChangePasswordRequest request);

    Task<bool> StartPasswordResetAsync(ForgotPasswordRequest request);

    Task<bool> ConfirmPasswordResetAsync(ConfirmForgotPasswordRequest request);
}
