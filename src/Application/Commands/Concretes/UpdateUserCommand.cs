using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.Extensions.Options;
using UsersService.Src.Application.Commands.Interfaces;
using UsersService.Src.Application.DTOs.Update;
using UsersService.Src.Application.Options;
using UsersService.Src.Domain.Interfaces;

namespace UsersService.Src.Application.Commands.Concretes;

public class UpdateUserCommand(
    IUserRepository userRepository,
    AmazonCognitoIdentityProviderClient cognitoClient,
    IOptions<CognitoSettings> cognitoOptions)
    : ICommand<(Guid PublicId, UpdateUserRequestDTO Request), bool>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly AmazonCognitoIdentityProviderClient _cognitoClient = cognitoClient;
    private readonly CognitoSettings _settings = cognitoOptions.Value;

    public async Task<bool> ExecuteAsync((Guid PublicId, UpdateUserRequestDTO Request) input)
    {
        var (publicId, request) = input;
        var user = await _userRepository.GetByPublicIdAsync(publicId);
        if (user == null)
        {
            return false;
        }

        if (request.Name != null)
        {
            user.Name = request.Name;
        }

        if (request.Phone != null)
        {
            user.Phone = request.Phone;
        }

        if (request.PhotoFileUrl != null)
        {
            user.PhotoFileUrl = request.PhotoFileUrl;
        }

        await _userRepository.SaveChangesAsync();

        var updateRequest = new AdminUpdateUserAttributesRequest
        {
            Username = user.Id.ToString(),
            UserPoolId = _settings.UserPoolId,
            UserAttributes = new List<AttributeType>(),
        };

        if (request.Name != null)
        {
            updateRequest.UserAttributes.Add(new AttributeType { Name = "name", Value = request.Name });
        }

        if (request.Phone != null)
        {
            updateRequest.UserAttributes.Add(new AttributeType { Name = "phone_number", Value = request.Phone });
        }

        await _cognitoClient.AdminUpdateUserAttributesAsync(updateRequest);

        return true;
    }
}
