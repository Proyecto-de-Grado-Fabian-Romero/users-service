namespace UsersService.Src.Application.Options;

public class CognitoSettings
{
    public string UserPoolId { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;
}
