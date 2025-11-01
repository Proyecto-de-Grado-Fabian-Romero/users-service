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
    IOptions<CognitoSettings> cognitoOptions
) : ICommand<(Guid PublicId, UpdateUserRequestDTO Request), bool>
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

        // Construir correctamente la lista de atributos
        var attributes = new List<AttributeType>();

        if (request.Name != null)
        {
            attributes.Add(new AttributeType { Name = "name", Value = request.Name });
        }

        if (request.Phone != null)
        {
            // Solo añadir prefijo si no viene con + (evita duplicar +591)
            var phoneValue = request.Phone.StartsWith("+") ? request.Phone : $"+591{request.Phone}";
            attributes.Add(new AttributeType { Name = "phone_number", Value = phoneValue });
        }

        if (request.PhotoFileUrl != null)
        {
            // Atributo común para foto de perfil en Cognito: "picture"
            attributes.Add(new AttributeType { Name = "picture", Value = request.PhotoFileUrl });
        }

        // Solo llamar a Cognito si hay atributos para actualizar
        if (attributes.Count > 0)
        {
            var updateRequest = new AdminUpdateUserAttributesRequest
            {
                Username = user.Id.ToString(), // asegúrate que esto coincide con el username en Cognito
                UserPoolId = _settings.UserPoolId,
                UserAttributes = attributes,
            };

            try
            {
                await _cognitoClient.AdminUpdateUserAttributesAsync(updateRequest);
            }
            catch (Amazon.CognitoIdentityProvider.Model.InvalidParameterException ex)
            {
                Console.WriteLine(
                    $"Warning: Cognito invalid parameter while updating user {user.Id}: {ex.Message}"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating Cognito attributes for user {user.Id}: {ex}");
            }
        }

        return true;
    }
}
