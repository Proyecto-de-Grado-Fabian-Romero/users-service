using System.IdentityModel.Tokens.Jwt;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using AutoMapper;
using UsersService.Src.Application.Commands.Interfaces;
using UsersService.Src.Application.DTOs;
using UsersService.Src.Domain.Interfaces;
using UsersService.Src.Infraestructure.Messaging;
using UsersService.Src.Infraestructure.Messaging.Contracts;

namespace UsersService.Src.Application.Commands.Concretes;

public class LoginUserCommand(
    AmazonCognitoIdentityProviderClient provider,
    CognitoUserPool userPool,
    IUserRepository repo,
    IMapper mapper,
    IFcmTokenPublisher fcmPublisher
) : ICommand<LoginRequest, LoggedUserDTO?>
{
    private readonly AmazonCognitoIdentityProviderClient _provider = provider;
    private readonly CognitoUserPool _userPool = userPool;
    private readonly IUserRepository _userRepository = repo;
    private readonly IMapper _mapper = mapper;
    private readonly IFcmTokenPublisher _fcmPublisher = fcmPublisher;

    public async Task<LoggedUserDTO?> ExecuteAsync(LoginRequest input)
    {
        var user = new CognitoUser(input.Email, _userPool.ClientID, _userPool, _provider);
        var authRequest = new InitiateSrpAuthRequest { Password = input.Password };

        try
        {
            var authResponse = await user.StartWithSrpAuthAsync(authRequest);
            if (authResponse.AuthenticationResult == null)
            {
                return null;
            }

            var appUser = await _userRepository.GetByIdAsync(Guid.Parse(user.Username));
            if (appUser == null || !appUser.VerifiedEmail)
            {
                return null;
            }

            // Map DTO de salida
            var dto = _mapper.Map<LoggedUserDTO>(appUser);
            dto.AccessToken = authResponse.AuthenticationResult.AccessToken;
            dto.RefreshToken = authResponse.AuthenticationResult.RefreshToken;

            // Si vino FCM token, publicarlo a AMQP
            if (!string.IsNullOrWhiteSpace(input.FcmToken))
            {
                var sessionId = ExtractJti(dto.AccessToken) ?? Guid.NewGuid().ToString("N");
                await _fcmPublisher.PublishAsync(
                    new TokenUpsertMessage
                    {
                        UserPublicId = dto.PublicId,
                        Token = input.FcmToken!,
                        SessionId = sessionId,
                        DeviceInfo = input.DeviceInfo,
                        ExpiresAt = null,
                    }
                );
            }

            return dto;
        }
        catch (UserNotConfirmedException)
        {
            throw new Exception("User's email is not confirmed.");
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static string? ExtractJti(string accessToken)
    {
        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            return jwt.Id; // jti
        }
        catch
        {
            return null;
        }
    }
}
