using System;
using UsersService.Src.Application.DTOs;

namespace UsersService.Src.Application.Interfaces;

public interface IUserService
{
    Task<UserDTO?> GetByPublicIdAsync(Guid publicId);

    Task<LoggedUserDTO?> LoginAsync(string email, string password);

    Task<LoggedUserDTO?> GetUserFromAccessTokenAsync(string accessToken);

    Task<string?> RefreshAccessTokenAsync(string refreshToken);

    Task<bool> IsAccessTokenValidAsync(string accessToken);
}
