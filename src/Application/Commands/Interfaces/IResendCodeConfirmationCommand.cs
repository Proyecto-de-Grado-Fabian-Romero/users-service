namespace UsersService.src.Application.Commands.Interfaces;

public interface IResendConfirmationCodeCommand
{
    Task<bool> ExecuteAsync(string email);
}
